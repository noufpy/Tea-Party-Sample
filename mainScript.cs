using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;

public class mainScript : MonoBehaviour {

	public Transform granny;
	public Transform claire;
	public Transform aj;
	public Transform kaya;
	public GameObject table;
	public Transform gorillaObj;
	public Transform cupcakesObj;
	public Transform malcolm;
	public Transform outerDoor;
	public Transform innerDoor;
	public Transform chairs;
	public Transform dishes;
	public GameObject winter;
	public GameObject terrain;
	public Light dirlight;
	private List<Vector3> chairPos = new List<Vector3>();
	private List<Quaternion> chairRot = new List<Quaternion>();
	private List<Vector3> dishesPos = new List<Vector3>();
	private List<Quaternion> dishesRot = new List<Quaternion>();
	private List<Vector3> cupcakesPos = new List<Vector3>();
	private List<Quaternion> cupcakesRot = new List<Quaternion>();

	Animator ajAnimator;
	Animator claireAnimator;
	Animator grannyAnimator;
	Animator kayaAnimator;

	private GameObject gorilla;
	private GameObject cupcakes;
	private GameObject door;
	private GameObject women;
	public GameObject textCanvas;
	public Transform malcolms;

	private Vector3 initialPosition_granny;
	private Quaternion initialRotation_granny;

	private Vector3 initialPosition_claire;
	private Quaternion initialRotation_claire;

	private Vector3 initialPosition_aj;
	private Quaternion initialRotation_aj;

	private Vector3 initialPosition_gorillaObj;
	private Quaternion initialRotation_gorillaObj;

	private Vector3 initialPosition_malcolm;
	private Quaternion initialRotation_malcolm;

	private Vector3 initialPosition_kaya;
	private Quaternion initialRotation_kaya;

	private Vector3 initialPosition_outerDoor;
	private Quaternion initialRotation_outerDoor;

	private Vector3 initialPosition_innerDoor;
	private Quaternion initialRotation_innerDoor;

	private Vector3 initialPosition_table;
	private Quaternion initialRotation_table;
	private bool resetF = false;
	private bool resetH = false;
	private bool resetG = false;
	private bool resetD = false;
	private bool resetC = false;
	private bool resetW = false;

	// FLOAT

	private float sine = 0.0f;
	private int sw = 0; //switch needed during sine curve
	private float timer = 0.0f;
	private float xMovement;
	private float xtorque;    //turns the object on the x axis
	private float ytorque;    //turns the object on the y axis
	private float ztorque;	//turns the object on the z axis
	public float verticalSpeed = 1.0f;
	public float verticalDistance = 1.0f;
	public float horizontalSpeed = 1.0f;
	public float spinSpeed = 1.0f;
	private Rigidbody rb_aj;
	private Rigidbody rb_claire;
	private Rigidbody rb_granny;
	private Rigidbody rb_kaya;
	private Rigidbody rb_table;
	private Rigidbody rb_chair;
	private Rigidbody rb_dish;
	private Rigidbody rb_cupcake;


	SerialPort sp = new SerialPort("/dev/cu.usbmodem1421",115200);
	float timeLeft = 30.0f;
	int serialNum;

	// Use this for initialization
	void Start () {

		int lastPos = PlayerPrefs.GetInt("servoPos");
		//print ( "Last Value Was " +  lastPos.ToString() );

		// ANIMATORS
		claireAnimator = claire.GetComponent<Animator> ();
		grannyAnimator = granny.GetComponent<Animator> ();
		ajAnimator = aj.GetComponent<Animator> ();
		kayaAnimator = kaya.GetComponent<Animator> ();

		// FIND LAYERS UNDER MAIN

		gorilla = GameObject.Find("gorillaDestroys");
		door = GameObject.Find("Door");
		cupcakes = GameObject.Find("hiddenCupcakes");
		women = GameObject.Find("Women");

		// SAVE POSITIONS

		initialPosition_aj = aj.transform.position;
		initialPosition_claire = claire.transform.position;
		initialPosition_granny = granny.transform.position;
		initialPosition_table = table.transform.position;
		initialPosition_gorillaObj = gorillaObj.transform.position;
		initialPosition_malcolm = malcolm.transform.position;
		initialPosition_outerDoor = outerDoor.transform.position;
		initialPosition_innerDoor = innerDoor.transform.position;
		initialPosition_kaya = kaya.transform.position;

		initialRotation_aj = aj.transform.rotation;
		initialRotation_claire = claire.transform.rotation;
		initialRotation_granny = granny.transform.rotation;
		initialRotation_table = table.transform.rotation;
		initialRotation_gorillaObj = gorillaObj.transform.rotation;
		initialRotation_malcolm = malcolm.transform.rotation;
		initialRotation_outerDoor = outerDoor.transform.rotation;
		initialRotation_innerDoor = innerDoor.transform.rotation;
		initialRotation_kaya = kaya.transform.rotation;

		for (int i = 0; i < chairs.transform.childCount; i++) {
			Transform kid = chairs.transform.GetChild (i);
			chairPos.Add(kid.transform.position);
			chairRot.Add(kid.transform.rotation);
		}

		for (int i = 0; i < dishes.transform.childCount; i++) {
			Transform dish = dishes.transform.GetChild (i);
			dishesPos.Add(dish.transform.position);
			dishesRot.Add(dish.transform.rotation);
		}

		for (int i = 0; i < cupcakesObj.transform.childCount; i++) {
			Transform cupcakeChild = cupcakesObj.transform.GetChild (i);
			cupcakesPos.Add(cupcakeChild.transform.position);
			cupcakesRot.Add(cupcakeChild.transform.rotation);
		}


		// FLOAT

		xMovement = Random.Range (-.5f, .5f) * horizontalSpeed;   //random value between -0.5 and 0.5, causing some movement on the x axis.
		xtorque = Random.Range (-5.0f, 5.0f) * spinSpeed;	//turns the object on the x axis
		ytorque = Random.Range (-5.0f, 5.0f) * spinSpeed;	//turns the object on the y axis
		ztorque = Random.Range (-5.0f, 5.0f) * spinSpeed;	//turns the object on the z axis

		sp.Open();
		sp.ReadTimeout = 5;
		foreach (Transform child in gameObject.transform) {
			child.gameObject.SetActive (false);
		}
	}

	public static Text AddTextToCanvas(string textString, GameObject canvasGameObject)
	{
		Text text = canvasGameObject.AddComponent<Text>();
		text.text = textString;

		Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		text.font = ArialFont;
		text.fontSize = 40;
		text.material = ArialFont.material;

		return text;
	}

	// Update is called once per frame
	void Update () {

		if (sp.IsOpen)
		{
			print ("serial is open(arduino been restarted)");

			try
			{
				serialNum = sp.ReadByte ();
				timeLeft = 30f;
			}
			catch (System.Exception)
			{
				print ("error");
			}
		}
		TurnOn(serialNum);
	}

	void TurnOn(int Value)
	{
		PlayerPrefs.SetInt("servoPos", Value);
		//print(timeLeft);
		print(Value);

		if (Value == 5 & timeLeft > 0) {
			// CUPCAKE RAIN ANIMATION

			if (aj.transform.position != initialPosition_aj & resetC == false) {
				resetF = false;
				resetH = false;
				resetG = false;
				resetD = false;
				resetW = false;

				women.SetActive (true);
				gorilla.SetActive (false);
				door.SetActive (false);
				winter.SetActive (false);
				terrain.SetActive (true);
				dirlight.GetComponent<Light> ().intensity = 1.8f;
				Destroy (textCanvas.GetComponent<Text> (), 0f);

				claireAnimator.Play ("Sitting");
				grannyAnimator.Play ("Sitting");
				ajAnimator.Play ("Sitting");
				kayaAnimator.Play ("Sitting");

				//aj
				aj.transform.position = initialPosition_aj;
				aj.transform.rotation = initialRotation_aj;

				//claire
				claire.transform.position = initialPosition_claire;
				claire.transform.rotation = initialRotation_claire;

				//granny
				granny.transform.position = initialPosition_granny;
				granny.transform.rotation = initialRotation_granny;

				//table
				table.transform.position = initialPosition_table;
				table.transform.rotation = initialRotation_table;

				//gorilla
				gorillaObj.transform.position = initialPosition_gorillaObj;
				gorillaObj.transform.rotation = initialRotation_gorillaObj;

				//malcolm
				malcolm.transform.position = initialPosition_malcolm;
				malcolm.transform.rotation = initialRotation_malcolm;

				//kaya
				kaya.transform.position = initialPosition_kaya;
				kaya.transform.rotation = initialRotation_kaya;
//
//				//door
//				outerDoor.transform.position = initialPosition_outerDoor;
//				outerDoor.transform.rotation = initialRotation_outerDoor;
//
//				innerDoor.transform.position = initialPosition_innerDoor;
//				innerDoor.transform.rotation = initialRotation_innerDoor;

				//chairs
				for (int i = 0; i < chairs.transform.childCount; i++) {
					Transform chairChild = chairs.transform.GetChild (i);
					chairChild.transform.position = chairPos [i];
					chairChild.transform.rotation = chairRot [i];
				}

				//dishes
				for (int i = 0; i < dishes.transform.childCount; i++) {
					Transform dishChild = dishes.transform.GetChild (i);
					dishChild.transform.position = dishesPos [i];
					dishChild.transform.rotation = dishesRot [i];
				}

				//cupcakes
				for (int i = 0; i < cupcakesObj.transform.childCount; i++) {
					Transform cupcakeChild = cupcakesObj.transform.GetChild (i);
					cupcakeChild.transform.position = cupcakesPos [i];
					cupcakeChild.transform.rotation = cupcakesRot [i];
				}

				// RESET FLOAT
				rb_aj = aj.GetComponent<Rigidbody> ();
				rb_aj.useGravity = true;
				rb_aj.mass = 100f;
				rb_aj.drag = 1f;
				rb_aj.angularDrag = 1f;

				rb_claire = claire.GetComponent<Rigidbody> ();
				rb_claire.useGravity = true;
				rb_claire.mass = 700f;
				rb_claire.drag = 100f;
				rb_claire.angularDrag = 10f;

				rb_granny = granny.GetComponent<Rigidbody> ();
				rb_granny.useGravity = true;
				rb_granny.mass = 700f;
				rb_granny.drag = 100f;
				rb_granny.angularDrag = 10f;

				rb_kaya = kaya.GetComponent<Rigidbody> ();
				rb_kaya.useGravity = true;
				rb_kaya.mass = 700f;
				rb_kaya.drag = 100f;
				rb_kaya.angularDrag = 10f;

				rb_table = table.GetComponent<Rigidbody> ();
				rb_table.useGravity = true;
				rb_table.mass = 100f;
				rb_table.drag = 20f;
				rb_table.angularDrag = 10f;

				//chairs
				foreach (Transform child in chairs.transform) {
					rb_chair = child.GetComponent<Rigidbody> ();
					rb_chair.useGravity = true;
					rb_chair.mass = 10f;
					rb_chair.drag = 0f;
					rb_chair.angularDrag = 0.05f;
				}

				//dishes
				foreach (Transform child in dishes.transform) {
					rb_dish = child.GetComponent<Rigidbody> ();
					rb_dish.useGravity = true;
					rb_dish.mass = 3f;
					rb_dish.drag = 3f;
					rb_dish.angularDrag = 3f;
				}

				//cupcakes
				foreach (Transform child in cupcakesObj.transform) {
					rb_cupcake = child.GetComponent<Rigidbody> ();
					rb_cupcake.useGravity = true;
					rb_cupcake.mass = 3f;
					rb_cupcake.drag = 1f;
					rb_cupcake.angularDrag = 2f;
				}

				resetC = true;
			}

			claireAnimator.Play ("Salsa Dancing");
			grannyAnimator.Play ("Salsa Dancing");
			ajAnimator.Play ("Salsa Dancing");
			kayaAnimator.Play ("Salsa Dancing");

			cupcakes.SetActive (true);
			timeLeft -= Time.deltaTime;


		} else if (Value == 4 & timeLeft > 0) {
			// GORILLA ANIMATION

			if (aj.transform.position != initialPosition_aj & resetG == false) {
				resetF = false;
				resetH = false;
				resetC = false;
				resetD = false;
				resetW = false;

				women.SetActive (true);
				cupcakes.SetActive (false);
				winter.SetActive (false);
				terrain.SetActive (true);
				dirlight.GetComponent<Light> ().intensity = 1.8f;

				claireAnimator.Play ("Sitting");
				grannyAnimator.Play ("Sitting");
				ajAnimator.Play ("Sitting");
				kayaAnimator.Play ("Sitting");
				door.SetActive (false);
				Destroy (textCanvas.GetComponent<Text> (), 0f);

				//aj
				aj.transform.position = initialPosition_aj;
				aj.transform.rotation = initialRotation_aj;

				//claire
				claire.transform.position = initialPosition_claire;
				claire.transform.rotation = initialRotation_claire;

				//granny
				granny.transform.position = initialPosition_granny;
				granny.transform.rotation = initialRotation_granny;

				//table
				table.transform.position = initialPosition_table;
				table.transform.rotation = initialRotation_table;

				//gorilla
				gorillaObj.transform.position = initialPosition_gorillaObj;
				gorillaObj.transform.rotation = initialRotation_gorillaObj;

				//malcolm
				malcolm.transform.position = initialPosition_malcolm;
				malcolm.transform.rotation = initialRotation_malcolm;

				//kaya
				kaya.transform.position = initialPosition_kaya;
				kaya.transform.rotation = initialRotation_kaya;
//
//				//door
//				outerDoor.transform.position = initialPosition_outerDoor;
//				outerDoor.transform.rotation = initialRotation_outerDoor;
//
//				innerDoor.transform.position = initialPosition_innerDoor;
//				innerDoor.transform.rotation = initialRotation_innerDoor;

				//chairs
				for (int i = 0; i < chairs.transform.childCount; i++) {
					Transform chairChild = chairs.transform.GetChild (i);
					chairChild.transform.position = chairPos [i];
					chairChild.transform.rotation = chairRot [i];
				}

				//dishes
				for (int i = 0; i < dishes.transform.childCount; i++) {
					Transform dishChild = dishes.transform.GetChild (i);
					dishChild.transform.position = dishesPos [i];
					dishChild.transform.rotation = dishesRot [i];
				}

				//cupcakes
				for (int i = 0; i < cupcakesObj.transform.childCount; i++) {
					Transform cupcakeChild = cupcakesObj.transform.GetChild (i);
					cupcakeChild.transform.position = cupcakesPos [i];
					cupcakeChild.transform.rotation = cupcakesRot [i];
				}

				// RESET FLOAT
				rb_aj = aj.GetComponent<Rigidbody> ();
				rb_aj.useGravity = true;
				rb_aj.mass = 100f;
				rb_aj.drag = 1f;
				rb_aj.angularDrag = 1f;

				rb_claire = claire.GetComponent<Rigidbody> ();
				rb_claire.useGravity = true;
				rb_claire.mass = 700f;
				rb_claire.drag = 100f;
				rb_claire.angularDrag = 10f;

				rb_granny = granny.GetComponent<Rigidbody> ();
				rb_granny.useGravity = true;
				rb_granny.mass = 700f;
				rb_granny.drag = 100f;
				rb_granny.angularDrag = 10f;

				rb_kaya = kaya.GetComponent<Rigidbody> ();
				rb_kaya.useGravity = true;
				rb_kaya.mass = 700f;
				rb_kaya.drag = 100f;
				rb_kaya.angularDrag = 10f;

				rb_table = table.GetComponent<Rigidbody> ();
				rb_table.useGravity = true;
				rb_table.mass = 100f;
				rb_table.drag = 20f;
				rb_table.angularDrag = 10f;

				//chairs
				foreach (Transform child in chairs.transform) {
					rb_chair = child.GetComponent<Rigidbody> ();
					rb_chair.useGravity = true;
					rb_chair.mass = 10f;
					rb_chair.drag = 0f;
					rb_chair.angularDrag = 0.05f;
				}

				//dishes
				foreach (Transform child in dishes.transform) {
					rb_dish = child.GetComponent<Rigidbody> ();
					rb_dish.useGravity = true;
					rb_dish.mass = 3f;
					rb_dish.drag = 3f;
					rb_dish.angularDrag = 3f;
				}

				//cupcakes
				foreach (Transform child in cupcakesObj.transform) {
					rb_cupcake = child.GetComponent<Rigidbody> ();
					rb_cupcake.useGravity = true;
					rb_cupcake.mass = 3f;
					rb_cupcake.drag = 1f;
					rb_cupcake.angularDrag = 2f;
				}

				resetG = true;
			}

			gorilla.SetActive (true);
			timeLeft -= Time.deltaTime;

		} else if (Value == 1 & timeLeft > 0) {
			// ERASE WOMEN ANIMATION

			if (aj.transform.position != initialPosition_aj & resetH == false) {
				resetF = false;
				resetG = false;
				resetC = false;
				resetD = false;
				resetW = false;

				claireAnimator.Play ("Sitting");
				grannyAnimator.Play ("Sitting");
				ajAnimator.Play ("Sitting");
				kayaAnimator.Play ("Sitting");

				gorilla.SetActive (false);
				cupcakes.SetActive (false);
				door.SetActive (false);
				winter.SetActive (false);
				terrain.SetActive (true);
				dirlight.GetComponent<Light> ().intensity = 1.8f;

				//aj
				aj.transform.position = initialPosition_aj;
				aj.transform.rotation = initialRotation_aj;

				//claire
				claire.transform.position = initialPosition_claire;
				claire.transform.rotation = initialRotation_claire;

				//granny
				granny.transform.position = initialPosition_granny;
				granny.transform.rotation = initialRotation_granny;

				//table
				table.transform.position = initialPosition_table;
				table.transform.rotation = initialRotation_table;

				//gorilla
				gorillaObj.transform.position = initialPosition_gorillaObj;
				gorillaObj.transform.rotation = initialRotation_gorillaObj;

				//malcolm
				malcolm.transform.position = initialPosition_malcolm;
				malcolm.transform.rotation = initialRotation_malcolm;

				//kaya
				kaya.transform.position = initialPosition_kaya;
				kaya.transform.rotation = initialRotation_kaya;
//
//				//door
//				outerDoor.transform.position = initialPosition_outerDoor;
//				outerDoor.transform.rotation = initialRotation_outerDoor;
//
//				innerDoor.transform.position = initialPosition_innerDoor;
//				innerDoor.transform.rotation = initialRotation_innerDoor;

				//chairs
				for (int i = 0; i < chairs.transform.childCount; i++) {
					Transform chairChild = chairs.transform.GetChild (i);
					chairChild.transform.position = chairPos [i];
					chairChild.transform.rotation = chairRot [i];
				}

				//dishes
				for (int i = 0; i < dishes.transform.childCount; i++) {
					Transform dishChild = dishes.transform.GetChild (i);
					dishChild.transform.position = dishesPos [i];
					dishChild.transform.rotation = dishesRot [i];
				}

				//cupcakes
				for (int i = 0; i < cupcakesObj.transform.childCount; i++) {
					Transform cupcakeChild = cupcakesObj.transform.GetChild (i);
					cupcakeChild.transform.position = cupcakesPos [i];
					cupcakeChild.transform.rotation = cupcakesRot [i];
				}

				// RESET FLOAT
				rb_aj = aj.GetComponent<Rigidbody> ();
				rb_aj.useGravity = true;
				rb_aj.mass = 100f;
				rb_aj.drag = 1f;
				rb_aj.angularDrag = 1f;

				rb_claire = claire.GetComponent<Rigidbody> ();
				rb_claire.useGravity = true;
				rb_claire.mass = 700f;
				rb_claire.drag = 100f;
				rb_claire.angularDrag = 10f;

				rb_granny = granny.GetComponent<Rigidbody> ();
				rb_granny.useGravity = true;
				rb_granny.mass = 700f;
				rb_granny.drag = 100f;
				rb_granny.angularDrag = 10f;

				rb_kaya = kaya.GetComponent<Rigidbody> ();
				rb_kaya.useGravity = true;
				rb_kaya.mass = 700f;
				rb_kaya.drag = 100f;
				rb_kaya.angularDrag = 10f;

				rb_table = table.GetComponent<Rigidbody> ();
				rb_table.useGravity = true;
				rb_table.mass = 100f;
				rb_table.drag = 20f;
				rb_table.angularDrag = 10f;

				//chairs
				foreach (Transform child in chairs.transform) {
					rb_chair = child.GetComponent<Rigidbody> ();
					rb_chair.useGravity = true;
					rb_chair.mass = 10f;
					rb_chair.drag = 0f;
					rb_chair.angularDrag = 0.05f;
				}

				//dishes
				foreach (Transform child in dishes.transform) {
					rb_dish = child.GetComponent<Rigidbody> ();
					rb_dish.useGravity = true;
					rb_dish.mass = 3f;
					rb_dish.drag = 3f;
					rb_dish.angularDrag = 3f;
				}

				//cupcakes
				foreach (Transform child in cupcakesObj.transform) {
					rb_cupcake = child.GetComponent<Rigidbody> ();
					rb_cupcake.useGravity = true;
					rb_cupcake.mass = 3f;
					rb_cupcake.drag = 1f;
					rb_cupcake.angularDrag = 2f;
				}

				resetH = true;
			}

			women.SetActive (false);
			timeLeft -= Time.deltaTime;
			ajAnimator.Play ("Dancing Running Man");
			kayaAnimator.Play ("Dancing Running Man");
			AddTextToCanvas ("Mission Accomplished: Women are erased", textCanvas);
		} else if (Value == 6 & timeLeft > 0) {
			// WHITE MEN ANIMATION
			if (aj.transform.position != initialPosition_aj & resetD == false) {
				resetH = false;
				resetG = false;
				resetC = false;
				resetF = false;
				resetW = false;

				cupcakes.SetActive (false);
				gorilla.SetActive (false);
				winter.SetActive (false);
				terrain.SetActive (true);
				dirlight.GetComponent<Light> ().intensity = 1.8f;

				claireAnimator.Play ("Sitting");
				grannyAnimator.Play ("Sitting");
				ajAnimator.Play ("Sitting");
				kayaAnimator.Play ("Sitting");

				women.SetActive (true);
				Destroy (textCanvas.GetComponent<Text> (), 0f);
				//Destroy(malcolms,0f);

				//aj
				aj.transform.position = initialPosition_aj;
				aj.transform.rotation = initialRotation_aj;

				//claire
				claire.transform.position = initialPosition_claire;
				claire.transform.rotation = initialRotation_claire;

				//granny
				granny.transform.position = initialPosition_granny;
				granny.transform.rotation = initialRotation_granny;

				//table
				table.transform.position = initialPosition_table;
				table.transform.rotation = initialRotation_table;

				//gorilla
				gorillaObj.transform.position = initialPosition_gorillaObj;
				gorillaObj.transform.rotation = initialRotation_gorillaObj;

				//malcolm
				malcolm.transform.position = initialPosition_malcolm;
				malcolm.transform.rotation = initialRotation_malcolm;

				//kaya
				kaya.transform.position = initialPosition_kaya;
				kaya.transform.rotation = initialRotation_kaya;
////
////				//door
//				outerDoor.transform.position = initialPosition_outerDoor;
//				outerDoor.transform.rotation = initialRotation_outerDoor;
//
//				innerDoor.transform.position = initialPosition_innerDoor;
//				innerDoor.transform.rotation = initialRotation_innerDoor;

				//chairs
				for (int i = 0; i < chairs.transform.childCount; i++) {
					Transform chairChild = chairs.transform.GetChild (i);
					chairChild.transform.position = chairPos [i];
					chairChild.transform.rotation = chairRot [i];
				}

				//dishes
				for (int i = 0; i < dishes.transform.childCount; i++) {
					Transform dishChild = dishes.transform.GetChild (i);
					dishChild.transform.position = dishesPos [i];
					dishChild.transform.rotation = dishesRot [i];
				}

				//cupcakes
				for (int i = 0; i < cupcakesObj.transform.childCount; i++) {
					Transform cupcakeChild = cupcakesObj.transform.GetChild (i);
					cupcakeChild.transform.position = cupcakesPos [i];
					cupcakeChild.transform.rotation = cupcakesRot [i];
				}

				// RESET FLOAT
				rb_aj = aj.GetComponent<Rigidbody> ();
				rb_aj.useGravity = true;
				rb_aj.mass = 100f;
				rb_aj.drag = 1f;
				rb_aj.angularDrag = 1f;

				rb_claire = claire.GetComponent<Rigidbody> ();
				rb_claire.useGravity = true;
				rb_claire.mass = 700f;
				rb_claire.drag = 100f;
				rb_claire.angularDrag = 10f;

				rb_granny = granny.GetComponent<Rigidbody> ();
				rb_granny.useGravity = true;
				rb_granny.mass = 700f;
				rb_granny.drag = 100f;
				rb_granny.angularDrag = 10f;

				rb_kaya = kaya.GetComponent<Rigidbody> ();
				rb_kaya.useGravity = true;
				rb_kaya.mass = 700f;
				rb_kaya.drag = 100f;
				rb_kaya.angularDrag = 10f;

				rb_table = table.GetComponent<Rigidbody> ();
				rb_table.useGravity = true;
				rb_table.mass = 100f;
				rb_table.drag = 20f;
				rb_table.angularDrag = 10f;

				//chairs
				foreach (Transform child in chairs.transform) {
					rb_chair = child.GetComponent<Rigidbody> ();
					rb_chair.useGravity = true;
					rb_chair.mass = 10f;
					rb_chair.drag = 0f;
					rb_chair.angularDrag = 0.05f;
				}

				//dishes
				foreach (Transform child in dishes.transform) {
					rb_dish = child.GetComponent<Rigidbody> ();
					rb_dish.useGravity = true;
					rb_dish.mass = 3f;
					rb_dish.drag = 3f;
					rb_dish.angularDrag = 3f;
				}

				//cupcakes
				foreach (Transform child in cupcakesObj.transform) {
					rb_cupcake = child.GetComponent<Rigidbody> ();
					rb_cupcake.useGravity = true;
					rb_cupcake.mass = 3f;
					rb_cupcake.drag = 1f;
					rb_cupcake.angularDrag = 2f;
				}
				resetD = true;

			}
			door.SetActive (true);
			timeLeft -= Time.deltaTime;
			// DOOR OPENING
//			if (innerDoor.transform.eulerAngles.y > 90 & innerDoor.transform.position.y < 20) {
//				innerDoor.transform.rotation = Quaternion.Slerp (innerDoor.transform.rotation, Quaternion.Euler (0, -20, 0), Time.deltaTime * 2f);
//				innerDoor.transform.position = new Vector3 (470.95f, outerDoor.transform.position.y, 670.2f);
//			}

		} else if (Value == 2 & timeLeft > 0) {
			// WINTER ANIMATION

			if (aj.transform.position != initialPosition_aj & resetW == false) {
				resetH = false;
				resetG = false;
				resetC = false;
				resetF = false;
				resetD = false;

				cupcakes.SetActive (false);
				gorilla.SetActive (false);
				door.SetActive (false);

				claireAnimator.Play ("Sitting");
				grannyAnimator.Play ("Sitting");
				ajAnimator.Play ("Sitting");
				kayaAnimator.Play ("Sitting");

				women.SetActive (true);
				Destroy (textCanvas.GetComponent<Text> (), 0f);
				//Destroy(malcolms,0f);

				//aj
				aj.transform.position = initialPosition_aj;
				aj.transform.rotation = initialRotation_aj;

				//claire
				claire.transform.position = initialPosition_claire;
				claire.transform.rotation = initialRotation_claire;

				//granny
				granny.transform.position = initialPosition_granny;
				granny.transform.rotation = initialRotation_granny;

				//table
				table.transform.position = initialPosition_table;
				table.transform.rotation = initialRotation_table;

				//gorilla
				gorillaObj.transform.position = initialPosition_gorillaObj;
				gorillaObj.transform.rotation = initialRotation_gorillaObj;

				//malcolm
				malcolm.transform.position = initialPosition_malcolm;
				malcolm.transform.rotation = initialRotation_malcolm;

				//kaya
				kaya.transform.position = initialPosition_kaya;
				kaya.transform.rotation = initialRotation_kaya;

				//chairs
				for (int i = 0; i < chairs.transform.childCount; i++) {
					Transform chairChild = chairs.transform.GetChild (i);
					chairChild.transform.position = chairPos [i];
					chairChild.transform.rotation = chairRot [i];
				}

				//dishes
				for (int i = 0; i < dishes.transform.childCount; i++) {
					Transform dishChild = dishes.transform.GetChild (i);
					dishChild.transform.position = dishesPos [i];
					dishChild.transform.rotation = dishesRot [i];
				}

				//cupcakes
				for (int i = 0; i < cupcakesObj.transform.childCount; i++) {
					Transform cupcakeChild = cupcakesObj.transform.GetChild (i);
					cupcakeChild.transform.position = cupcakesPos [i];
					cupcakeChild.transform.rotation = cupcakesRot [i];
				}

				// RESET FLOAT
				rb_aj = aj.GetComponent<Rigidbody> ();
				rb_aj.useGravity = true;
				rb_aj.mass = 100f;
				rb_aj.drag = 1f;
				rb_aj.angularDrag = 1f;

				rb_claire = claire.GetComponent<Rigidbody> ();
				rb_claire.useGravity = true;
				rb_claire.mass = 700f;
				rb_claire.drag = 100f;
				rb_claire.angularDrag = 10f;

				rb_granny = granny.GetComponent<Rigidbody> ();
				rb_granny.useGravity = true;
				rb_granny.mass = 700f;
				rb_granny.drag = 100f;
				rb_granny.angularDrag = 10f;

				rb_kaya = kaya.GetComponent<Rigidbody> ();
				rb_kaya.useGravity = true;
				rb_kaya.mass = 700f;
				rb_kaya.drag = 100f;
				rb_kaya.angularDrag = 10f;

				rb_table = table.GetComponent<Rigidbody> ();
				rb_table.useGravity = true;
				rb_table.mass = 100f;
				rb_table.drag = 20f;
				rb_table.angularDrag = 10f;

				//chairs
				foreach (Transform child in chairs.transform) {
					rb_chair = child.GetComponent<Rigidbody> ();
					rb_chair.useGravity = true;
					rb_chair.mass = 10f;
					rb_chair.drag = 0f;
					rb_chair.angularDrag = 0.05f;
				}

				//dishes
				foreach (Transform child in dishes.transform) {
					rb_dish = child.GetComponent<Rigidbody> ();
					rb_dish.useGravity = true;
					rb_dish.mass = 3f;
					rb_dish.drag = 3f;
					rb_dish.angularDrag = 3f;
				}

				//cupcakes
				foreach (Transform child in cupcakesObj.transform) {
					rb_cupcake = child.GetComponent<Rigidbody> ();
					rb_cupcake.useGravity = true;
					rb_cupcake.mass = 3f;
					rb_cupcake.drag = 1f;
					rb_cupcake.angularDrag = 2f;
				}
				resetW = true;

			}
			winter.SetActive (true);
			claireAnimator.Play ("Yelling");
			grannyAnimator.Play ("Yelling");
			ajAnimator.Play ("Yelling");
			kayaAnimator.Play ("Yelling");
			terrain.SetActive (false);
			dirlight.GetComponent<Light> ().intensity = 0.3f;
			timeLeft -= Time.deltaTime;

		} else if (Value == 3 & timeLeft > 0) {
			// FLOAT ANIMATION

			if (aj.transform.position != initialPosition_aj & resetF == false) {
				resetH = false;
				resetG = false;
				resetC = false;
				resetD = false;
				resetW = false;

				door.SetActive (false);
				cupcakes.SetActive (false);
				gorilla.SetActive (false);
				winter.SetActive (false);
				terrain.SetActive (true);
				dirlight.GetComponent<Light> ().intensity = 1.8f;

				claireAnimator.Play ("Sitting");
				grannyAnimator.Play ("Sitting");
				ajAnimator.Play ("Sitting");
				kayaAnimator.Play ("Sitting");

				women.SetActive (true);
				Destroy(textCanvas.GetComponent<Text>(),0f);

				//aj
				aj.transform.position = initialPosition_aj;
				aj.transform.rotation = initialRotation_aj;

				//claire
				claire.transform.position = initialPosition_claire;
				claire.transform.rotation = initialRotation_claire;

				//granny
				granny.transform.position = initialPosition_granny;
				granny.transform.rotation = initialRotation_granny;

				//table
				table.transform.position = initialPosition_table;
				table.transform.rotation = initialRotation_table;

				//gorilla
				gorillaObj.transform.position = initialPosition_gorillaObj;
				gorillaObj.transform.rotation = initialRotation_gorillaObj;

				//malcolm
				malcolm.transform.position = initialPosition_malcolm;
				malcolm.transform.rotation = initialRotation_malcolm;

				//kaya
				kaya.transform.position = initialPosition_kaya;
				kaya.transform.rotation = initialRotation_kaya;
//
//				//door
//				outerDoor.transform.position = initialPosition_outerDoor;
//				outerDoor.transform.rotation = initialRotation_outerDoor;
//
//				innerDoor.transform.position = initialPosition_innerDoor;
//				innerDoor.transform.rotation = initialRotation_innerDoor;

				//chairs
				for (int i = 0; i < chairs.transform.childCount; i++) {
					Transform chairChild = chairs.transform.GetChild (i);
					chairChild.transform.position = chairPos[i];
					chairChild.transform.rotation = chairRot[i];
				}

				//dishes
				for (int i = 0; i < dishes.transform.childCount; i++) {
					Transform dishChild = dishes.transform.GetChild (i);
					dishChild.transform.position = dishesPos[i];
					dishChild.transform.rotation = dishesRot[i];
				}

				//cupcakes
				for (int i = 0; i < cupcakesObj.transform.childCount; i++) {
					Transform cupcakeChild = cupcakesObj.transform.GetChild (i);
					cupcakeChild.transform.position = cupcakesPos[i];
					cupcakeChild.transform.rotation = cupcakesRot[i];
				}
				resetF = true;
			}

			rb_aj = aj.GetComponent<Rigidbody>();
			rb_aj.useGravity = false;
			rb_aj.mass = 1f;
			rb_aj.drag = 0f;
			rb_aj.angularDrag = 0.05f;

			rb_claire = claire.GetComponent<Rigidbody>();
			rb_claire.useGravity = false;
			rb_claire.mass = 1f;
			rb_claire.drag = 0f;
			rb_claire.angularDrag = 0.05f;

			rb_granny = granny.GetComponent<Rigidbody>();
			rb_granny.useGravity = false;
			rb_granny.mass = 1f;
			rb_granny.drag = 0f;
			rb_granny.angularDrag = 0.05f;

			rb_kaya = kaya.GetComponent<Rigidbody>();
			rb_kaya.useGravity = false;
			rb_kaya.mass = 1f;
			rb_kaya.drag = 0f;
			rb_kaya.angularDrag = 0.05f;

			rb_table = table.GetComponent<Rigidbody>();
			rb_table.useGravity = false;
			rb_table.mass = 1f;
			rb_table.drag = 0f;
			rb_table.angularDrag = 0.05f;

			//chairs
			foreach (Transform child in chairs.transform) {
				rb_chair = child.GetComponent<Rigidbody>();
				rb_chair.useGravity = false;
				rb_chair.mass = 1f;
				rb_chair.drag = 0f;
				rb_chair.angularDrag = 0.05f;
			}

			//dishes
			foreach (Transform child in dishes.transform) {
				rb_dish = child.GetComponent<Rigidbody>();
				rb_dish.useGravity = false;
				rb_dish.mass = 1f;
				rb_dish.drag = 0f;
				rb_dish.angularDrag = 0.05f;
			}

			//cupcakes
			foreach (Transform child in cupcakesObj.transform) {
				rb_cupcake = child.GetComponent<Rigidbody>();
				rb_cupcake.useGravity = false;
				rb_cupcake.mass = 1f;
				rb_cupcake.drag = 0f;
				rb_cupcake.angularDrag = 0.05f;
			}

			if (sine < Mathf.PI && sw == 0) {	//sine variable is fluctuating between 0 and Pi causing an up and down motion simulating floating, think sine curve
				sine += Time.deltaTime;
			}
			if (sine >= Mathf.PI) {
				sw = 1;
			}
			if (sine <= 0) {
				sw = 0;
			}
			if (sine >= 0 && sw == 1) {
				sine = 0;
			}

			rb_aj.velocity = new Vector3 (xMovement, Mathf.Sin (2 * sine * verticalSpeed) * verticalDistance, 0);	//Adds the x axis movement and up and down motion to the object
			rb_claire.velocity = new Vector3 (xMovement, Mathf.Sin (2 * sine * verticalSpeed) * verticalDistance, 0);
			rb_granny.velocity = new Vector3 (xMovement, Mathf.Sin (2 * sine * verticalSpeed) * verticalDistance, 0);
			rb_table.velocity = new Vector3 (xMovement, Mathf.Sin (2 * sine * verticalSpeed) * verticalDistance, 0);
			rb_kaya.velocity = new Vector3 (xMovement, Mathf.Sin (2 * sine * verticalSpeed) * verticalDistance, 0);

			foreach (Transform child in chairs.transform) {
				rb_chair = child.GetComponent<Rigidbody>();
				rb_chair.velocity = new Vector3 (xMovement, Mathf.Sin (2 * sine * verticalSpeed) * verticalDistance, 0);
			}
			foreach (Transform child in dishes.transform) {
				rb_dish.velocity = new Vector3 (xMovement, Mathf.Sin (2 * sine * verticalSpeed) * verticalDistance, 0);
			}
			foreach (Transform child in cupcakesObj.transform) {
				rb_cupcake.velocity = new Vector3 (xMovement, Mathf.Sin (2 * sine * verticalSpeed) * verticalDistance, 0);
			}

			if (timer < 10) { //increments timer
				timer += Time.deltaTime;
			}
			if (timer >= 10) {    //This adds the torque that was executed at the start again every 10 seconds to have the object continue to turn slightly.
				timer = 0;
				rb_aj.AddRelativeTorque (new Vector3 (xtorque, ytorque, ztorque));	//Adds the torque on all axis again. Does not compute new numbers just continues previous ones.
				rb_claire.AddRelativeTorque (new Vector3 (xtorque, ytorque, ztorque));
				rb_granny.AddRelativeTorque (new Vector3 (xtorque, ytorque, ztorque));
				rb_table.AddRelativeTorque (new Vector3 (xtorque, ytorque, ztorque));
				rb_kaya.AddRelativeTorque (new Vector3 (xtorque, ytorque, ztorque));

				foreach (Transform child in chairs.transform) {
					rb_chair = child.GetComponent<Rigidbody>();
					rb_chair.AddRelativeTorque (new Vector3 (xtorque, ytorque, ztorque));
				}
				foreach (Transform child in dishes.transform) {
					rb_dish.AddRelativeTorque (new Vector3 (xtorque, ytorque, ztorque));
				}
				foreach (Transform child in cupcakesObj.transform) {
					rb_cupcake.AddRelativeTorque (new Vector3 (xtorque, ytorque, ztorque));
				}
			}

			claireAnimator.Play ("Floating");
			grannyAnimator.Play ("Floating");
			ajAnimator.Play ("Floating");
			kayaAnimator.Play ("Floating");
			timeLeft -= Time.deltaTime;

		} else {
			serialNum = 0;
			timeLeft = 30f;
			//print ("I am else");

			cupcakes.SetActive (false);
			gorilla.SetActive (false);
			door.SetActive (false);
			women.SetActive (true);
			winter.SetActive (false);
			terrain.SetActive (true);
			dirlight.GetComponent<Light> ().intensity = 1.8f;
			Destroy(textCanvas.GetComponent<Text>(),0f);

			// RESET FLOAT
			rb_aj = aj.GetComponent<Rigidbody>();
			rb_aj.useGravity = true;
			rb_aj.mass = 100f;
			rb_aj.drag = 1f;
			rb_aj.angularDrag = 1f;

			rb_claire = claire.GetComponent<Rigidbody>();
			rb_claire.useGravity = true;
			rb_claire.mass = 700f;
			rb_claire.drag = 100f;
			rb_claire.angularDrag = 10f;

			rb_granny = granny.GetComponent<Rigidbody>();
			rb_granny.useGravity = true;
			rb_granny.mass = 700f;
			rb_granny.drag = 100f;
			rb_granny.angularDrag = 10f;

			rb_kaya = kaya.GetComponent<Rigidbody> ();
			rb_kaya.useGravity = true;
			rb_kaya.mass = 700f;
			rb_kaya.drag = 100f;
			rb_kaya.angularDrag = 10f;

			rb_table = table.GetComponent<Rigidbody>();
			rb_table.useGravity = true;
			rb_table.mass = 100f;
			rb_table.drag = 20f;
			rb_table.angularDrag = 10f;

			//chairs
			foreach (Transform child in chairs.transform) {
				rb_chair = child.GetComponent<Rigidbody>();
				rb_chair.useGravity = true;
				rb_chair.mass = 10f;
				rb_chair.drag = 0f;
				rb_chair.angularDrag = 0.05f;
			}

			//dishes
			foreach (Transform child in dishes.transform) {
				rb_dish = child.GetComponent<Rigidbody>();
				rb_dish.useGravity = true;
				rb_dish.mass = 3f;
				rb_dish.drag = 3f;
				rb_dish.angularDrag = 3f;
			}

			//cupcakes
			foreach (Transform child in cupcakesObj.transform) {
				rb_cupcake = child.GetComponent<Rigidbody>();
				rb_cupcake.useGravity = true;
				rb_cupcake.mass = 3f;
				rb_cupcake.drag = 1f;
				rb_cupcake.angularDrag = 2f;
			}

			claireAnimator.Play ("Sitting");
			grannyAnimator.Play ("Sitting");
			ajAnimator.Play ("Sitting");
			kayaAnimator.Play ("Sitting");

			// RESET

			//aj
			aj.transform.position = initialPosition_aj;
			aj.transform.rotation = initialRotation_aj;

			//claire
			claire.transform.position = initialPosition_claire;
			claire.transform.rotation = initialRotation_claire;

			//granny
			granny.transform.position = initialPosition_granny;
			granny.transform.rotation = initialRotation_granny;

			//table
			table.transform.position = initialPosition_table;
			table.transform.rotation = initialRotation_table;

			//gorilla
			gorillaObj.transform.position = initialPosition_gorillaObj;
			gorillaObj.transform.rotation = initialRotation_gorillaObj;

			//malcolm
			malcolm.transform.position = initialPosition_malcolm;
			malcolm.transform.rotation = initialRotation_malcolm;

			//kaya
			kaya.transform.position = initialPosition_kaya;
			kaya.transform.rotation = initialRotation_kaya;

			//door
//			outerDoor.transform.position = initialPosition_outerDoor;
//			outerDoor.transform.rotation = initialRotation_outerDoor;
//
//			innerDoor.transform.position = initialPosition_innerDoor;
//			innerDoor.transform.rotation = initialRotation_innerDoor;

			//chairs
			for (int i = 0; i < chairs.transform.childCount; i++) {
				Transform chairChild = chairs.transform.GetChild (i);
				chairChild.transform.position = chairPos[i];
				chairChild.transform.rotation = chairRot[i];
			}

			//dishes
			for (int i = 0; i < dishes.transform.childCount; i++) {
				Transform dishChild = dishes.transform.GetChild (i);
				dishChild.transform.position = dishesPos[i];
				dishChild.transform.rotation = dishesRot[i];
			}

			//cupcakes
			for (int i = 0; i < cupcakesObj.transform.childCount; i++) {
				Transform cupcakeChild = cupcakesObj.transform.GetChild (i);
				cupcakeChild.transform.position = cupcakesPos[i];
				cupcakeChild.transform.rotation = cupcakesRot[i];
			}
			}

		}
	}
