using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class PushAgentSix : Agent
{
    // agent health
	AgentHealth agentHealth;
    
    // character animation
    AnimateMe AnimationController;
   
    public GameObject ground;
    public GameObject area;
    //public Bounds areaBounds;
    public GameObject goal;
    GoalDetect GoalDetect;
    ChangeGroundMaterial ChangeGround;
    public bool useVectorObs;
    public int speed;

    Rigidbody AgentRb;  //cached on initialization

    EnvironmentParameters m_ResetParams;

    void Awake(){
        agentHealth = GetComponent<AgentHealth> ();
        AnimationController = GetComponent<AnimateMe>();
        GoalDetect = goal.GetComponent<GoalDetect>();
        ChangeGround = goal.GetComponent<ChangeGroundMaterial>();
        speed = 350;
    }

    public override void Initialize()
    {
        // Cache the agent rigidbody
        AgentRb = GetComponent<Rigidbody>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;

    }


    /// Called when the agent moves the block into the goal.
    public void ScoredAGoal()
    {
        
        // We use a reward of 5.
        AddReward(5f);

        // By marking an agent as done AgentReset() will be called automatically.
        if(GoalDetect.goalCount > 5) {
            EndEpisode();
        }
        
        // reset goal 
        GoalDetect.goal = false;

        // Swap ground material for a bit to indicate we scored.
        // thus script is attached to the goal now
        StartCoroutine(ChangeGround.GoalScoredSwapGroundMaterial(ChangeGround.goalScoredMaterial, 0.5f));
    }


    // Branches Size 1 // Branch 0 size 4 // possible outcomes 0,1,2,3
    // angular drag increased to 5 from 0.05
    // trained on PushBlockTwelve (slow speed 80 / small cube / v small goal / high ownGoal penalty) 
    // resume with
    // mlagents-learn trainer_config_push_cube.yaml --run-id=PushBlockNo --force
    public void MoveAgent(float[] act)
    {
        
        //var speed = 80f;
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = Mathf.FloorToInt(act[0]);

        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 0.7f;
                break;
            case 2:
                rotateDir = transform.up * 0.25f;
                break;
            case 3:
                rotateDir = transform.up * -0.25f;
                break;
        }
        
        // only included for training
        // transform.Rotate (rotateDir, Time.fixedDeltaTime * 200.0f);
		// AgentRb.AddForce (dirToGo * speed, ForceMode.Force);

        // animate agent
        AnimationController.UpdateAnimatorDirection(AnimationController.calculateDirection());
        AnimationController.UpdateAnimator(AnimationController.CalculateSpeed());

		//move agent
		//if health over 200 keep moving
		if(agentHealth.Health >= 200f) {
            AnimationController.IsPanting = false;
			transform.Rotate (rotateDir, Time.fixedDeltaTime * 200.0f);
			AgentRb.AddForce (dirToGo * speed, ForceMode.Force);
		} 
		// if less than 200 pant
		else if(agentHealth.Health < 200.0f && agentHealth.Health >= 100.0f) {
			AnimationController.IsPanting = true;
		}
		// if less than 100 prone
		else if(agentHealth.Health < 100.0f) {
            //AnimationController.IsPanting = false;
			AnimationController.MakeProne ();
		}

    }


    /// Called every step of the engine. Here the agent takes an action.
    public override void OnActionReceived(float[] vectorAction)
    {
        
        // Move the agent using the action.
        MoveAgent(vectorAction);

        // Penalty given each step to encourage agent to finish task quickly.
        AddReward(-1f / MaxStep);

        // receive reward of 5pts for a goal
        if(GoalDetect.goal) {
            ScoredAGoal();
        }

        // lose pts if cube / block comes out of goal
        if(GoalDetect.ownGoal) {
            AddReward(-3f);
            //Debug.Log("own goal");
            // reset ownGoal
            GoalDetect.ownGoal = false;
        }
    }

    // test manually
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = 0;
        if (Input.GetKey(KeyCode.W))
        {
            actionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            actionsOut[0] = 2;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            actionsOut[0] = 3;
        }
    }

    /// In the editor, if "Reset On Done" is checked then AgentReset() will be
    /// called automatically anytime we mark done = true in an agent script.
    public override void OnEpisodeBegin()
    {
        // rotate the area and change the position of the goal
        // var rotation = Random.Range(0, 4);
        // var rotationAngle = rotation * 90f;
        // area.transform.Rotate(new Vector3(0f, rotationAngle, 0f));

        //Reset agent
        transform.position = ground.transform.position + new Vector3(Random.Range(6, -6), 1f, Random.Range(6, -6));
        AgentRb.velocity = Vector3.zero;
        AgentRb.angularVelocity = Vector3.zero;


        // reset goal count
        GoalDetect.goal = false;
        // problem this counts all goals
        GoalDetect.goalCount = 0;

        // reset cubes / blocks
        // make list of all the cubes in each 'area' reset only agents area
        var AgentParent = transform.parent.name;
        GameObject parent = transform.parent.gameObject;
        List<GameObject> cubes = new List<GameObject>();

        foreach (Transform child in parent.transform) {
            if (child.tag == "cube") {
                cubes.Add(child.gameObject);
            }
        }

        
        //GameObject[] cubes = GameObject.FindGameObjectsWithTag("block");
        foreach (var cube in cubes) {
            // avoid the goal
            // if x is greater than less than -14
            // z is less than 6 and greater than 0
            int x = Random.Range(-19, 19);
            int z = Random.Range(-19, 19);
            if (x < -14 && z < 6 && z > 0) {
                x += 6;
            }
            cube.transform.position = ground.transform.position + new Vector3(x, 1f, z);
        }
    }


}
