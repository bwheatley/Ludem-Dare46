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


    public class FishJob {
        public Transform transform;
        public float     moveY;
        public float     moveX;
    }

    // Start is called before the first frame update
    void Start()
    {

        //Spawn in the fishies
        fishList = new List<FishJob>();
        for (int i = 0; i < 1000; i++) {
            Transform fishTransform = Instantiate(pfFish[Random.Range(0,pfFish.Count)],
                new Vector3(Random.Range(GameManager.instance.Map_BL.x, GameManager.instance.Map_BR.x), Random.Range(GameManager.instance.Map_TL.y, GameManager.instance.Map_BL.y)),
                quaternion.identity);
            fishList.Add(
                new FishJob {
                    transform = fishTransform, //There location
                    moveY     = Random.Range(.5f, 5f), //Their speed in the Y direction
                    moveX     = Random.Range(.5f, 5f)  //Their speed in the X direction
                }
            );
        }

        //Spawn in the baddies
        enemiesList = new List<FishJob>();
        for (int i = 0; i < 5; i++) {
            Transform enemiesTransform = Instantiate(pfEnemies[Random.Range(0, pfEnemies.Count)],
                new Vector3(Random.Range(GameManager.instance.Map_BL.x,  GameManager.instance.Map_BR.x), Random.Range(GameManager.instance.Map_TL.y, GameManager.instance.Map_BL.y)),
                quaternion.identity);
            enemiesList.Add(
                new FishJob {
                    transform = enemiesTransform,         //There location
                    moveY     = Random.Range(.5f, 5f), //Their speed in the Y direction
                    moveX     = Random.Range(.5f, 5f)  //Their speed in the X direction
                }
            );
        }


    }

    // Update is called once per frame
    void Update() {
        float startTime = Time.realtimeSinceStartup;

        // NativeArray<float3> position = new NativeArray<float3>(fishList.Count, Allocator.TempJob);
        TransformAccessArray transformAccessArray = new TransformAccessArray(fishList.Count);
        NativeArray<float>   moveY                = new NativeArray<float>(fishList.Count, Allocator.TempJob);
        NativeArray<float>   moveX                = new NativeArray<float>(fishList.Count, Allocator.TempJob);

        //Fill the arrays up with our current data
        for (int i = 0; i < fishList.Count; i++) {
            transformAccessArray.Add(fishList[i].transform);
            moveY[i] = fishList[i].moveY;
            moveX[i] = fishList[i].moveX;
        }

        //
        FishParallelJobTransform colonistParallelJobTransform = new FishParallelJobTransform();
        colonistParallelJobTransform.deltaTime = Time.deltaTime;
        colonistParallelJobTransform.moveY     = moveY;
        colonistParallelJobTransform.moveX     = moveX;
        colonistParallelJobTransform.mapTop = GameManager.instance.Map_TL.y;
        colonistParallelJobTransform.mapBottom = GameManager.instance.Map_BL.y;
        colonistParallelJobTransform.mapLeft = GameManager.instance.Map_TL.x;
        colonistParallelJobTransform.mapRight = GameManager.instance.Map_BR.x;



        JobHandle jobHandle = colonistParallelJobTransform.Schedule(transformAccessArray);
        jobHandle.Complete();

        //Update the original transforms
        for (int i = 0; i < fishList.Count; i++) {
            fishList[i].moveY = moveY[i];
            fishList[i].moveX = moveX[i];
        }

        //We must clean up our nativearrays or memleaks will occur
        // position.Dispose();
        transformAccessArray.Dispose();
        moveY.Dispose();


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

        public void Execute(int index, TransformAccess transform) {
            transform.position += new Vector3(moveX[index] * deltaTime, moveY[index] * deltaTime, 0f);

            //Top Edge
            if (transform.position.y >= mapTop) {
                moveY[index] = -math.abs(moveY[index]);
            }

            //Bottom Edge
            if (transform.position.y <= mapBottom) {
                moveY[index] = +math.abs(moveY[index]);
            }

            //Left Edge
            if (transform.position.x <= mapLeft) {
                //Send it right
                moveX[index] = +math.abs(moveX[index]);
            }


            //Right Edge
            if (transform.position.x >= mapRight) {
                //Send it left
                moveX[index] = -math.abs(moveX[index]);
            }


            //If the moveX < 0 we're going left, flip the sprite
            //STRETCH GOAL

        }
    }


}
