using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;


public class FishJobs : MonoBehaviour
{

    [SerializeField] private bool              useJobs;
    [SerializeField] private List<Transform>         pfFish;
    [SerializeField] private List<Transform>         pfEnemies;
    private                  List<FishJob> fishList;
    private                  List<FishJob> enemiesList;
    public int fishcount;
    public static FishJobs instance;
    public int enemyCount;
    [SerializeField]
    private float FlipMinTime = 5f;

    public class FishJob {
        public Transform transform;
        public float     moveY;
        public float     moveX;
        public float currentFlipTime;
    }


    void Awake () {
        //We only ever want 1 game manager
        if (instance == null ) {
            instance = this;
        }
        else if ( instance != this ) {
            Debug.Log( string.Format( "More then one copy of Gamemanager..i must end myself!" ) );
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        int _difficulty = 0;
        //Determine enemy count from difficulty
        if (Difficulty.instance != null) {
            _difficulty = Difficulty.instance.GetDifficulty();
        }

        var _EnemySpeedBase = 2f;
        var _EnemySpeedMax  = 5f;

        switch (_difficulty) {
            case 0: //Easy * .25 = 5
                enemyCount = Mathf.RoundToInt(GameManager.instance.GetDifficultyMupliplier(3) * (float)enemyCount);
                break;
            case 1: //Normal .5 = 10
                enemyCount = Mathf.RoundToInt(GameManager.instance.GetDifficultyMupliplier(2) * (float)enemyCount);
                _EnemySpeedBase = _EnemySpeedBase * 2f;
                _EnemySpeedMax = _EnemySpeedMax * 2f;
                break;
            case 2: //Hard * 1 = 20
                enemyCount = Mathf.RoundToInt(GameManager.instance.GetDifficultyMupliplier(1) * (float)enemyCount);
                _EnemySpeedBase = _EnemySpeedBase * 2.5f;
                _EnemySpeedMax  = _EnemySpeedMax  * 2.5f;
                break;
            case 3: //Impossible * 2 = 40
                enemyCount = Mathf.RoundToInt(GameManager.instance.GetDifficultyMupliplier(0) * (float)enemyCount);
                _EnemySpeedBase = _EnemySpeedBase * 3f;
                _EnemySpeedMax  = _EnemySpeedMax  * 3f;
                break;
        }





        //Spawn in the fishies
        fishList = new List<FishJob>();
        for (int i = 0; i < fishcount; i++) {
            Transform fishTransform = Instantiate(pfFish[Random.Range(0,pfFish.Count)],
                new Vector3(Random.Range(GameManager.instance.Map_BL.x, GameManager.instance.Map_BR.x), Random.Range(GameManager.instance.Map_TL.y, GameManager.instance.Map_BL.y)),
                quaternion.identity);

            if (i % 2 == 0) {
                fishTransform.localScale = new Vector3(fishTransform.localScale.x, fishTransform.localScale.y, fishTransform.localScale.z);
                fishList.Add(
                    new FishJob {
                        transform = fishTransform,         //There location
                        moveY     = Random.Range(.5f, 5f), //Their speed in the Y direction
                        moveX     = Random.Range(.5f, 5f),  //Their speed in the X direction
                        currentFlipTime = 0f
                    }
                );
            }
            else {
                fishTransform.localScale = new Vector3(-fishTransform.localScale.x, fishTransform.localScale.y, fishTransform.localScale.z);
                fishList.Add(
                    new FishJob {
                        transform = fishTransform,         //There location
                        moveY     = Random.Range(-.5f, -5f), //Their speed in the Y direction
                        moveX     = Random.Range(-.5f, -5f),  //Their speed in the X direction
                        currentFlipTime = 0f
                    }
                );
            }
        }

        //Spawn in the baddies
        enemiesList = new List<FishJob>();
        for (int i = 0; i < enemyCount; i++) {
            Transform enemiesTransform = Instantiate(pfEnemies[Random.Range(0, pfEnemies.Count)],
                new Vector3(Random.Range(GameManager.instance.Map_BL.x,  GameManager.instance.Map_BR.x), Random.Range(GameManager.instance.Map_TL.y, GameManager.instance.Map_BL.y)),
                quaternion.identity);

            if (i % 2 == 0) {
                enemiesTransform.localScale = new Vector3(enemiesTransform.localScale.x, enemiesTransform.localScale.y, enemiesTransform.localScale.z);
                enemiesList.Add(
                    new FishJob {
                        transform = enemiesTransform,     //There location
                        moveY     = Random.Range(_EnemySpeedBase, _EnemySpeedMax), //Their speed in the Y direction
                        moveX     = Random.Range(_EnemySpeedBase, _EnemySpeedMax),  //Their speed in the X direction
                        currentFlipTime = 0f
                    }
                );
            }
            else {
                enemiesTransform.localScale = new Vector3(-enemiesTransform.localScale.x, enemiesTransform.localScale.y, enemiesTransform.localScale.z);
                enemiesList.Add(
                    new FishJob {
                        transform = enemiesTransform,     //There location
                        moveY     = Random.Range(_EnemySpeedBase, -_EnemySpeedMax), //Their speed in the Y direction
                        moveX     = Random.Range(_EnemySpeedBase, -_EnemySpeedMax),  //Their speed in the X direction
                        currentFlipTime = 0f
                    }
                );
            }
        }


    }

    // Update is called once per frame
    void Update() {
        float startTime = Time.realtimeSinceStartup;


        TransformAccessArray transformAccessArray = new TransformAccessArray(fishList.Count);
        NativeArray<float>   moveY                = new NativeArray<float>(fishList.Count, Allocator.TempJob);
        NativeArray<float>   moveX                = new NativeArray<float>(fishList.Count, Allocator.TempJob);
        NativeArray<float>   FlipCurrentTime      = new NativeArray<float>(fishList.Count, Allocator.TempJob);

        //Fill the arrays up with our current data
        for (int i = 0; i < fishList.Count; i++) {
            transformAccessArray.Add(fishList[i].transform);
            moveY[i] = fishList[i].moveY;
            moveX[i] = fishList[i].moveX;
            FlipCurrentTime[i] = fishList[i].currentFlipTime;
        }

        //Create the Fishies Job
        FishParallelJobTransform colonistParallelJobTransform = new FishParallelJobTransform();
        colonistParallelJobTransform.deltaTime = Time.deltaTime;
        colonistParallelJobTransform.moveY     = moveY;
        colonistParallelJobTransform.moveX     = moveX;
        colonistParallelJobTransform.mapTop = GameManager.instance.Map_TL.y;
        colonistParallelJobTransform.mapBottom = GameManager.instance.Map_BL.y;
        colonistParallelJobTransform.mapLeft = GameManager.instance.Map_TL.x;
        colonistParallelJobTransform.mapRight = GameManager.instance.Map_BR.x;
        colonistParallelJobTransform.currentFlipTime = FlipCurrentTime;
        colonistParallelJobTransform.FlipMinTime = FlipMinTime;

        JobHandle jobHandle = colonistParallelJobTransform.Schedule(transformAccessArray);
        jobHandle.Complete();
        //End the fishies job

        //Start the enemies Job
        TransformAccessArray enemyTransformAccessArray = new TransformAccessArray(enemiesList.Count);
        NativeArray<float>   enemyMoveY                = new NativeArray<float>(enemiesList.Count, Allocator.TempJob);
        NativeArray<float>   enemyMoveX                = new NativeArray<float>(enemiesList.Count, Allocator.TempJob);
        NativeArray<float> enemyFlipCurrentTime = new NativeArray<float>(enemiesList.Count, Allocator.TempJob);

        //Fill the arrays up with our current data
        for (int i = 0; i < enemiesList.Count; i++) {
            enemyTransformAccessArray.Add(enemiesList[i].transform);
            enemyMoveY[i] = enemiesList[i].moveY;
            enemyMoveX[i] = enemiesList[i].moveX;
            enemyFlipCurrentTime[i] = fishList[i].currentFlipTime;
        }

        //Create the Sharkies Job
        FishParallelJobTransform enemyParallelJobTransform = new FishParallelJobTransform();
        enemyParallelJobTransform.deltaTime = Time.deltaTime;
        enemyParallelJobTransform.moveY     = enemyMoveY;
        enemyParallelJobTransform.moveX     = enemyMoveX;
        enemyParallelJobTransform.mapTop    = GameManager.instance.Map_TL.y;
        enemyParallelJobTransform.mapBottom = GameManager.instance.Map_BL.y;
        enemyParallelJobTransform.mapLeft   = GameManager.instance.Map_TL.x;
        enemyParallelJobTransform.mapRight  = GameManager.instance.Map_BR.x;
        enemyParallelJobTransform.currentFlipTime = FlipCurrentTime;
        enemyParallelJobTransform.FlipMinTime = FlipMinTime;

        JobHandle enemyJobHandle = enemyParallelJobTransform.Schedule(enemyTransformAccessArray);
        enemyJobHandle.Complete();
        //End the enemies job

        //Update the original transforms
        for (int i = 0; i < fishList.Count; i++) {
            fishList[i].moveY = moveY[i];
            fishList[i].moveX = moveX[i];
            fishList[i].currentFlipTime = FlipCurrentTime[i];
        }
        //Update the enemies
        for (int i = 0; i < enemiesList.Count; i++) {
            enemiesList[i].moveY = enemyMoveY[i];
            enemiesList[i].moveX = enemyMoveX[i];
            enemiesList[i].currentFlipTime = enemyFlipCurrentTime[i];
        }

        //We must clean up our nativearrays or memleaks will occur
        // position.Dispose();
        transformAccessArray.Dispose();
        moveY.Dispose();
        moveX.Dispose();
        FlipCurrentTime.Dispose();

        enemyTransformAccessArray.Dispose();
        enemyMoveY.Dispose();
        enemyMoveX.Dispose();
        enemyFlipCurrentTime.Dispose();

    }

    [BurstCompile]
    public struct FishParallelJobTransform : IJobParallelForTransform {
        public NativeArray<float> moveY;
        public NativeArray<float> moveX;
        public float              deltaTime;
        public float mapTop;
        public float mapBottom;
        public float mapLeft;
        public float mapRight;
        public NativeArray<float> currentFlipTime;
        public float FlipMinTime;

        public void Execute(int index, TransformAccess transform) {
            transform.position += new Vector3(moveX[index] * deltaTime, moveY[index] * deltaTime, 0f);

            if (currentFlipTime[index] > 0f) {
                currentFlipTime[index] -= deltaTime;
                //We will reset when we're in the job
            }


            //Top Edge
            if (transform.position.y >= mapTop-1) {
                moveY[index] = -math.abs(moveY[index]);
            }

            //Bottom Edge
            if (transform.position.y <= mapBottom+1) {
                moveY[index] = +math.abs(moveY[index]);
            }

            //Left Edge
            if (transform.position.x <= mapLeft+1) {
                //Send it right
                moveX[index] = +math.abs(moveX[index]);

                if (currentFlipTime[index] <= 0f) {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y,
                        transform.localScale.z);
                    //TODO reset this an pass back out of the job
                    currentFlipTime[index] = FlipMinTime;
                }
            }
            //Right Edge
            if (transform.position.x >= mapRight-1) {
                //Send it left
                moveX[index] = -math.abs(moveX[index]);

                //Check flip timer before flipping them off
                if (currentFlipTime[index] <= 0f) {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y,
                        transform.localScale.z);
                    //TODO reset this an pass back out of the job
                    currentFlipTime[index] = FlipMinTime;
                }

            }

            //If the moveX < 0 we're going left, flip the sprite
            if (moveX[index] < 0f) {

            }
            else {

            }
            //STRETCH GOAL
        }
    }

    /// <summary>
    /// Eat a fish, remove the fish from the game.
    /// Rebuild fishList
    /// </summary>
    /// <param name="_gameObject"></param>
    public void DeleteFish(GameObject _gameObject) {
        //Unique Id of the object
        _gameObject.SetActive(false);

        //Decrease the fish counter
        UiManager.instance.RemoveFish();
    }

    public void DeleteEnemy(GameObject _gameObject) {
        _gameObject.SetActive(false);

        //Decrease the fish counter
        UiManager.instance.RemoveEnemy();
    }



}
