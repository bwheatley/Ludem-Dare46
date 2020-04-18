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
    private                  List<FishJob> fishList;


    public class FishJob {
        public Transform transform;
        public float     moveY;
    }

    // Start is called before the first frame update
    void Start()
    {
        fishList = new List<FishJob>();
        for (int i = 0; i < 1000; i++) {
            Transform fishTransform = Instantiate(pfFish[Random.Range(0,pfFish.Count)],
                new Vector3(Random.Range(-8f, 8f), Random.Range(-5f, 5f)),
                quaternion.identity);
            fishList.Add(
                new FishJob {
                    transform = fishTransform,
                    moveY     = Random.Range(1f, 2f)
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

        //Fill the arrays up with our current data
        for (int i = 0; i < fishList.Count; i++) {
            // position[i] = fishList[i].transform.position;
            // transformAccessArray[i] = fishList[i].transform;
            transformAccessArray.Add(fishList[i].transform);
            moveY[i] = fishList[i].moveY;
        }

        //
        FishParallelJobTransform colonistParallelJobTransform = new FishParallelJobTransform();
        colonistParallelJobTransform.deltaTime = Time.deltaTime;
        colonistParallelJobTransform.moveY     = moveY;
        colonistParallelJobTransform.mapTop = GameManager.instance.Map_TL.y;
        colonistParallelJobTransform.mapBottom = GameManager.instance.Map_BL.y;
        colonistParallelJobTransform.mapLeft = GameManager.instance.Map_TL.x;
        colonistParallelJobTransform.mapRight = GameManager.instance.Map_BR.x;



        JobHandle jobHandle = colonistParallelJobTransform.Schedule(transformAccessArray);
        jobHandle.Complete();

        //Update the original transforms
        for (int i = 0; i < fishList.Count; i++) {
            // fishList[i].transform.position = position[i];
            fishList[i].moveY = moveY[i];
        }

        //We must clean up our nativearrays or memleaks will occur
        // position.Dispose();
        transformAccessArray.Dispose();
        moveY.Dispose();


    }

    [BurstCompile]
    public struct FishParallelJobTransform : IJobParallelForTransform {
        public NativeArray<float> moveY;
        public float              deltaTime;
        public float mapTop;
        public float mapBottom;
        public float mapLeft;
        public float mapRight;

        public void Execute(int index, TransformAccess transform) {
            transform.position += new Vector3(0, moveY[index] * deltaTime, 0f);

            //Top Edge
            if (transform.position.y >= mapTop) {
                moveY[index] = -math.abs(moveY[index]);
            }

            //Bottom Edge
            if (transform.position.y <= mapBottom) {
                moveY[index] = +math.abs(moveY[index]);
            }

            //Left Edge

            //Right Edge



        }
    }


}
