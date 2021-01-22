// TO DO

// Implemnet a health score for the agent
// attach to agent get value from goal
// Trigger IsPanting > Death on score
// ANIMATIONS
// fix walking running animation


// start ML agents
// cd to config folder
// mlagents-learn trainer_config_push_cube.yaml --run-id=PushBlockFourteen --force


//I'm learning how to use ML agents and have some success training cubes to learn tasks. Following the ML Agents examples and modding them.

//I'm now trying to swap out the agent / cube for a standard assets third person controller prefab. However as the third person controller doesn't move using rigidbody.AddForce the ML agents training behaviour doesn't seem to be able to move the agent / character, despite it all working perfectly using heuristic behaviour (in the behaviour parameters).

//I wondered if anyone had any tips for how to integrate an animated third person character into an ML environment. Is it possible with the standard assets third person controller prefab.

//In case any one is interested the code that is causing the problem is below.

//The code that the ML behaviour uses to move the agent is below this message. It receives 7 inputs and in the ML Agents example (push block) uses these to directly move the agents rigidbody using AddForce

     //transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
     //m_AgentRb.AddForce(dirToGo * m_PushBlockSettings.agentRunSpeed, ForceMode.VelocityChange);

//These lines work well and do successfully train a cube (or any other rigidbody agent).

//However when using the standard assets third person controller the agent is moved by calling the Move function from the third person controller script.

 //    Char_Move = Forward*Vector3.forward + Right*Vector3.right;
 //     // pass all parameters to the character control script
 //     m_Character.Move(Char_Move, Char_Crouch, Char_Jump);

//The character controller script doesn't seem to directly use AddForce. And the agent won't move during training (but is controllable by heuristic behaviour, keys WASD).

//The full function is below.

//Hoping somebody might have some pointers about how to go about this.