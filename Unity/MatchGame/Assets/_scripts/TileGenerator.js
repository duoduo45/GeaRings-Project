var numberOfTiles = 16;

var hit:RaycastHit; //used to get data back from a raycase

var tileObjects:GameObject[];

var matchOne: GameObject;
var matchTwo: GameObject;

var tileName1:String;
var tileName2:String;

var tName1:Array;
var tName2:Array;

var canClick = true;

var scoreInt = 0;
var scoreTxt:String;

var tileLocations = new Array
(
	Vector3(0, 0, 0), Vector3(1.5, 0, 0),
	Vector3(3 ,0, 0), Vector3(4.5, 0, 0),
	Vector3(0, 1.5, 0), Vector3(1.5, 1.5, 0),
	Vector3(3, 1.5, 0), Vector3(4.5, 1.5, 0),
	Vector3(0, 3, 0), Vector3(1.5, 3, 0),
	Vector3(3, 3, 0), Vector3(4.5, 3, 0),
	Vector3(0, 4.5, 0), Vector3(1.5, 4.5, 0),
	Vector3(3, 4.5, 0), Vector3(4.5, 4.5, 0)
);

private var startTime;
private var Seconds:int;
private var roundedSeconds:int;
private var txtSeconds:int;
private var txtMinutes:int;
private var stopTimer = false;

var countSeconds:int;

var egyptStyle:GUIStyle;

var finishedTexture:Texture2D;
var timeUpTexture:Texture2D;
var finished = false;
var timeUp = false;

function Start()
{
	Camera.main.transform.position = Vector3(2.25, 2.25, -10);
	
	RandFuncs.Shuffle(tileObjects);
	
	for (var i = 0; i < numberOfTiles; i++)
	{
		Instantiate (tileObjects[i], tileLocations[i], Quaternion.identity);
	}
}


function Update ()
{
	if (canClick == true)
	{
		if (Input.GetButtonDown("Fire1"))
		{
			/*a function that sets up our ray that will go from the main camera through a point in the screen that we identify. 
			The position of the screen point is a Vector, which in this case has benn declared as the position of  the mouse*/
			var ray1 = Camera.main.ScreenPointToRay(Input.mousePosition); 
			
			/*if the mouse is clicked, a ray is cast into the scene, and if the ray collides with an object (the object MUST have
			a physics collider attached) then do somthing else*/
			if (Physics.Raycast	(ray1, hit, Mathf.Infinity	))
			{
				if (!matchOne)
				{
					revealCardOne();
				}
				else
				{
					revealCardTwo();
				}
			}
		}
	}
}

function OnGUI()
{	
	scoreTxt = scoreInt.ToString();
	GUI.Label(Rect(360, 55, 100, 30), scoreTxt, egyptStyle);
	
	if (stopTimer == false)
	{
		var guiTime = Time.time - startTime; //Time.time - retrieve the time from the system
		Seconds = countSeconds - (guiTime);
	}
	
	if (Seconds == 0)
	{
		print("The time is over");
		stopTimer = true;
		timeUp = true;
	}
	
	//Display timer
	roundedSeconds = Mathf.CeilToInt(Seconds); //Mathf.CeilToInt to round the number to the nearest full integer.
	txtSeconds = roundedSeconds % 60;
	txtMinutes = roundedSeconds / 60;
	
	text = String.Format("{0:00}:{1:00}", txtMinutes, txtSeconds); //String.Format is used to create a format that integers can be placed in.
	GUI.Label(Rect(530, 55, 100, 30), text, egyptStyle);
	
	if (finished == true)
	{
		GUI.Label(Rect(270, 305, 512, 256), finishedTexture);
	}
	
	if (timeUp == true)
	{
		GUI.Label(Rect(270, 305, 512, 256), timeUpTexture);
	}

}

function Awake()
{
	startTime = 60;
}

function revealCardOne()
{
	matchOne = hit.transform.gameObject;
	tileName1 = matchOne.transform.parent.name;
	
	if (matchOne == null)
	{
		print ("No object found!");
	}
	else
	{
		tName1 = tileName1.Split("_"[0]);
		
		/*we are accessing the Game Objects stored in the variable matchOne
		and then accessing the parent's animation component attached to each of this*/
		matchOne.transform.parent.animation.Play("tileReveal");
		print (tName1[0]);
	}
}

function revealCardTwo()
{
	matchTwo = hit.transform.gameObject;
	tileName2 = matchTwo.transform.parent.name;
	
	if (tileName1 != tileName2)
	{
		
		if (matchTwo == null)
		{
			print ("No object found!");
		}
		else
		{
			tName2 = tileName2.Split("_"[0]);
			matchTwo.transform.parent.animation.Play("tileReveal");
			print (tName2[0]);
		}
		
		if (tName1[0] == tName2[0])
		{
			canClick = false;
			yield new WaitForSeconds(2); //pauses the running of the game by a set number of seconds
			
			// to remove our selected Game Objects from the scene.
			Destroy(matchOne);
			Destroy(matchTwo);
			
			canClick = true;
			//decrement the number of tiles by and use this number as an edn clause.
			numberOfTiles = numberOfTiles-2;
			
			scoreInt++;
			
			if (numberOfTiles <= 0)
			{
				print("End game");
				stopTimer = true;
				finished = true;
			}
		}
		else 
		{
			canClick = false;
			yield new WaitForSeconds(2);
			matchOne.transform.parent.animation.Play("tileHide");
			matchTwo.transform.parent.animation.Play("tileHide");
			
			canClick = true;
		}
		
		// this basically empties the variables and ensures that we avoid any conflicts between selected objects.
		matchOne = null;
		matchTwo = null;
	}
}