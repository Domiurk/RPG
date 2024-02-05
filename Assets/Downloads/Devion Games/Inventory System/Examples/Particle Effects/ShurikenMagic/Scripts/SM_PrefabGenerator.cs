using UnityEngine;

public class SM_PrefabGenerator : MonoBehaviour {
	public GameObject[] createThis;

	private float rndNr;

	public int thisManyTimes=3;
	public float overThisTime=1.0f;
	
	public float xWidth;
	public float yWidth;
	public float zWidth;
	
	public float xRotMax;
	public float yRotMax=180f;
	public float zRotMax;
	
	public bool allUseSameRotation;
	private bool allRotationDecided;
	
	public bool detachToWorld=true;
	
	private float x_cur;
	private float y_cur;
	private float z_cur;
	
	
	private float xRotCur;
	private float yRotCur;
	private float zRotCur;
	
	private float timeCounter;
	private int effectCounter;

	private float trigger;

	private void Start () {
		if (thisManyTimes<1) 
			thisManyTimes=1;
		trigger=(overThisTime/thisManyTimes);
	}
	
	
	private void Update () {
		
		timeCounter+=Time.deltaTime;
		
		if(timeCounter>trigger&&effectCounter<=thisManyTimes)
		{
			rndNr=Mathf.Floor(Random.value*createThis.Length);

			x_cur=transform.position.x+(Random.value*xWidth)-(xWidth*0.5f);
			y_cur=transform.position.y+(Random.value*yWidth)-(yWidth*0.5f);
			z_cur=transform.position.z+(Random.value*zWidth)-(zWidth*0.5f);
			
			if(allUseSameRotation==false||allRotationDecided==false){
				xRotCur=transform.rotation.x+(Random.value*xRotMax*2f)-(xRotMax);
				yRotCur=transform.rotation.y+(Random.value*yRotMax*2f)-(yRotMax);  
				zRotCur=transform.rotation.z+(Random.value*zRotMax*2f)-(zRotMax);  
				allRotationDecided=true;
			}

			
			GameObject justCreated=(GameObject)Instantiate(createThis[(int)rndNr], new Vector3(x_cur, y_cur, z_cur), transform.rotation);
			justCreated.transform.Rotate(xRotCur, yRotCur, zRotCur);
			
			if(detachToWorld==false){
				justCreated.transform.parent=transform;
			}
			
			timeCounter-=trigger;
			effectCounter+=1;
		}
		
		
	}
}
