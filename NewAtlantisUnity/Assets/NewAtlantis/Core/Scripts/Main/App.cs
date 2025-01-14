﻿using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;



public enum AppState
{
	Login,
	Register,
	Spaces,
	Space,
	Game,
	Asset
};


public enum NavigationMode
{
	Flying,
	Walking
};




public enum AppTab
{
	Lobby,
	User,
	Scene,
	Space,
	About,
	Options,
	Chat,
	None
};

public enum TypeTab
{
	Mine,
	SharedWithMe,
};

//New Atlantis Viewer Main App
public class App : MonoBehaviour 
{
	AppState state = AppState.Login;

	List<NAObject> listObjects = new List<NAObject>();
	string strSpace = "";
	NAObject currentSelection = null;
	NAObject currentLocal = null;
	public GameObject goPrefabCube; 
	//public GameObject goPrefabCubeSimple; 
	public GameObject goPrefabSphere; 
	public GameObject goPrefabAvatar; 

	
	/*
	WWW www = null;
	WWW wwwPost = null;
	List<WWW> 			requests 	= new List<WWW>();
	*/

	XmlDocument 		xml 		= null;
	XPathNavigator  	xpn			= null;
	XmlNamespaceManager xnm 		= null;
	Texture2D			texWhite 	= null;
	Camera 				mainCamera 	= null;
	Camera 				selectedCamera = null;
	public bool	 		bGUI 		= true;

	float 				timerGC		= 0;
	float 				timerRefresh		= 0;

	GameObject			goRootSpace = null;
	bool				loading		= false;

	private  FileInfo[] 	info = null;

	List<GameObject> 	cameras 	= new List<GameObject>();
	//List<GameObject>	player_objects = new List<GameObject>();


	Vector3				colorAvatar = Vector3.zero;
	string				strPick = "";


	private bool				bStartPopup = true;

	private string 				strIP = "88.178.228.172";
	private string 				strFile = "DummyObject.unity3d";

	GameObject goMainLight = null;
	//GameObject goAvatar = null;
	GameObject goGrab = null;
	Vector3		PreviousMousePosition = Vector3.zero;
	
	AppTab 		tab = AppTab.Lobby;
	AppTab[] 	tabs = {AppTab.Lobby, AppTab.User, AppTab.Space, AppTab.Scene, AppTab.Options, AppTab.Chat, AppTab.About};

	TypeTab 	tabAssets = TypeTab.Mine;
	TypeTab 	tabSpaces = TypeTab.Mine;
	TypeTab[] 	tabsAssets = {TypeTab.Mine, TypeTab.SharedWithMe};
	TypeTab[] 	tabsSpaces = {TypeTab.Mine, TypeTab.SharedWithMe};

	bool bDisplayAvatarNames = true;

	//"tools", à restructurer
	bool 	bFrotte			= false;
	GameObject goFrotte 	= null;
	bool 	bHit			= false;
	bool	bPushObjects 	= false;
	bool	bPullObjects 	= false;
	bool	bGrab 			= false;

	private NAToolBase[] tools;
	private int current_tool = 0;
	//cameras
	private NACamera[] camerascripts;
	private int current_camera = 0;

	GUIStyle style = new GUIStyle();
	//CHAT
	string strCurrentChatMessage = "";
	public string strName = "noname";
	string strObjectName = "object_name";

	static Vector2 WindowSize = new Vector2(790,530);
	Rect mGuiWinRectChat 		= new Rect(Screen.width-300, 200, 300, Screen.height-200);
	Rect mGuiWinRectNetwork 	= new Rect(Screen.width/2-200, Screen.height/2-250, 400, 500);
	
	Rect mGuiWinRectScene 		= new Rect(Screen.width/2-WindowSize.x/2, Screen.height/2-WindowSize.y/2, WindowSize.x, WindowSize.y);

	Rect mGuiWinRectSpaces 		= new Rect(Screen.width/2-WindowSize.x/2, Screen.height/2-WindowSize.y/2, WindowSize.x, WindowSize.y);
	Rect mGuiWinRectOptions 	= new Rect(600, Screen.height/2-200, 200, 400);
	Rect mGuiWinRectAbout 		= new Rect(800, Screen.height/2-200, 200, 400);
	Rect mGuiWinRectLogin 		= new Rect(Screen.width/2-150, Screen.height/2-150, 300, 300);
	Rect mGuiWinRectRegister 	= new Rect(Screen.width/2-150, Screen.height/2-150, 300, 300);
	Rect mGuiWinRectSpace 		= new Rect(Screen.width/2-200, Screen.height/2-200, 400, 400);
	Rect mGuiWinRectAsset 		= new Rect(Screen.width/2-200, Screen.height/2-200, 400, 400);
	Rect mGuiWinRectLobby 		= new Rect(Screen.width/2-WindowSize.x/2, Screen.height/2-WindowSize.y/2, WindowSize.x, WindowSize.y);
	Rect mGuiWinRectUser 		= new Rect(Screen.width/2-WindowSize.x/2, Screen.height/2-WindowSize.y/2, WindowSize.x, WindowSize.y);
	private Vector2 scrollPos 	= Vector2.zero;
	private Vector2 scrollPosMySpaces 		= Vector2.zero;
	private Vector2 scrollPosSharedSpaces 	= Vector2.zero;
	private Vector2 scrollPosMyAssets 		= Vector2.zero;
	private Vector2 scrollPosAssetFileDialog = Vector2.zero;
	private Vector2 scrollPosSharedAssets 	= Vector2.zero;
	private Vector2 scrollPosLobbySpaces 	= Vector2.zero;
	private Vector2 scrollPosSpace 			= Vector2.zero;

	private Vector2 scrollPosLights 			= Vector2.zero;
	private Vector2 scrollPosCameras 			= Vector2.zero;
	private Vector2 scrollPosSources 			= Vector2.zero;



	/*
	string 	strLogin 			= "";
	string 	strPassword 		= "";
	string 	strPasswordRetype 	= "";
	string 	strEmail 			= "";
	string 	strSpaceName 		= "";


	bool	bSpacePublic 		= true;
	bool	bAssetPublic 		= true;
	*/


	List<Space> 	listSpaces 	= new List<Space>();
	List<Asset> 	listAssets 	= new List<Asset>(); //Asset bundles library 
	Asset 			CurrentAsset = null;
	string			AssetFilter = "";
	string			SpaceFilter = "";
	HostData 		currentHost = null;


	// Use this for initialization
	void Start () 
	{
		Debug.Log ("IP="+MasterServer.ipAddress);
		//AssetBundlePreviewGenerator.Test("Bundles/grass_ground.unity3d");
		//AssetBundlePreviewGenerator.Test("Bundles/CubeRouge.unity3d");
		//AssetBundlePreviewGenerator.Test("Bundles/Lobby.unity3d");
		//AssetBundlePreviewGenerator.Test("Bundles/sea.unity3d");

		//AssetBundlePreviewGenerator.Test("Bundles/object_566998620efb62.87109271.unity3d");
		//AssetBundlePreviewGenerator.Test("Bundles/object_56699e44b6b8d2.24800951.unity3d");


		//AssetBundlePreviewGenerator.Test("Bundles/object_5669cdf72da595.13058200.unity3d");
		//AssetBundlePreviewGenerator.Test("Bundles/object_5669cde3e8d6c2.73707730.unity3d");
		//AssetBundlePreviewGenerator.Test("Bundles/object_5669cdf72da595.13058200.unity3d");
		//AssetBundlePreviewGenerator.Test("Bundles/object_5669cb9c015478.93079789.unity3d");

		//AssetBundlePreviewGenerator.Test("Bundles/sea.unity3d");
		NA.app 			= this;
		TransitionManager.Init();
		TransitionManager.Start(TransitionManager.FadeIn,3f,Color.black, null);
		Init();
		NAServer.strLogin 		= PlayerPrefs.GetString("login");
		NAServer.strPassword 	= PlayerPrefs.GetString("pwd");
		colorAvatar 	= new Vector3(Random.value, Random.value, Random.value);
		GameObject.DontDestroyOnLoad(gameObject);
		refreshHostList();
		//root of Space objects - must be at the origin
		goRootSpace 	= new GameObject("root_space");
		goRootSpace.transform.position = Vector3.zero;

		mainCamera 		= Camera.main;
		selectedCamera 	= mainCamera;
		NA.listener 	= mainCamera.GetComponent<AudioListener>();
		cameras.Add (Camera.main.gameObject);
		texWhite 		= Resources.Load ("white") as Texture2D;
		goMainLight 	= GameObject.Find ("MainLightViewer");
		ChatManager.Log("system", "welcome to New Atlantis", 0);
		NAToolBase[] _tools = GetComponents<NAToolBase>();

		ConnectionTesterStatus status = Network.TestConnection(false);
		LogManager.Log ("Network connection tests result=" + status.ToString());
		NA.PausePhysics();
	}



	void Init()
	{
		//tools = GetComponents<NAToolBase>();
		tools = GetComponentsInChildren<NAToolBase>();
		current_tool = 0;
		SetCurrentTool(tools[current_tool]);
		/*
		foreach (NAToolBase t in tools)
		{

		}
		*/
		//camerascripts = GetComponents<NACamera>();
		camerascripts = GetComponentsInChildren<NACamera>();
		current_camera = 0;
		SetCurrentCamera(camerascripts[current_camera]);
	}

	void SetCurrentTool(NAToolBase t)
	{
		foreach (NAToolBase tb in tools)
		{
			tb.enabled = (tb == t) ? true : false;
		}

	}

	void SetCurrentCamera(NACamera c)
	{
		foreach (NACamera cb in camerascripts)
		{
			cb.enabled = (cb == c) ? true : false;
		}
		
	}








	void CreateNetworkAvatar()
	{
		//goAvatar = InstantiateObject(goPrefabAvatar, Vector3.zero, Quaternion.identity, Vector3.one, 0);

		if (Network.isServer || Network.isClient)
		{
			//in all cases the current app keeps the avatar
			//goAvatar = Network.Instantiate(goPrefabAvatar, Vector3.zero, Quaternion.identity, 0) as GameObject;
			GetComponent<NetworkView>().RPC("SpawnAvatar", RPCMode.AllBuffered, Network.AllocateViewID(), colorAvatar, NAServer.strLogin);
		}
		else
		{
			//no need ?
			//goAvatar = InstantiateObject(goPrefabAvatar, Vector3.zero, Quaternion.identity, Vector3.one, 0);
		}
	}



	void UnactivateCameras()
	{
		Camera[] cams = Camera.FindObjectsOfType (typeof(Camera)) as Camera[];
		foreach (Camera c in cams)
		{
			bool bIgnore = false;
			if (c.transform.parent)
			{
				if (c.transform.parent.name == "TRS Gizmo" || c.transform.name == "Depth Camera")
				{
					bIgnore = true; //patch pour éviter que la caméra Gizmo soit désactivée
				}
			}
			if (c != selectedCamera && !bIgnore)
			{
				if (!cameras.Contains(c.gameObject))
				{
					cameras.Add (c.gameObject);
				}
				c.enabled = false;
				AudioListener listener = c.GetComponent<AudioListener>();
				if (listener != null)
				{
					listener.enabled = false;
				}
			}
		}

		AudioListener[] listeners = AudioListener.FindObjectsOfType (typeof(AudioListener)) as AudioListener[];
		foreach (AudioListener al in listeners)
		{
			if (al.gameObject.GetComponent<Camera>() != selectedCamera)
			{
				al.enabled = false;
			}
		}
	}



	void LateUpdate()
	{
		NA.SetAvatarPositionAndAngles(transform.position, transform.eulerAngles);

		bool bPlayPhysics = false;
		if (listObjects.Count > 0)
			bPlayPhysics = true;
		foreach (NAObject o in listObjects) 
		{
			if (o.downloading)
				bPlayPhysics = false;
		}
		if (currentSelection != null)
			bPlayPhysics = false;
		if (bPlayPhysics)
			NA.PlayPhysics();
		else
			NA.PausePhysics();
	}


	float GetLoadingProgress()
	{
		if (listObjects.Count == 0)
			return -1f;
		float loaded = 0f;
		foreach (NAObject o in listObjects) 
		{
			if (!o.downloading)
			loaded += 1f;
        }
		return loaded/(float)listObjects.Count;
	}






	// Update is called once per frame
	void Update () 
	{
		ObjectUploader.Process();
		NADownloader.Process();
		TransitionManager.Process();
		timerGC+=Time.deltaTime;

		/*timerRefresh+=Time.deltaTime;
		if (timerRefresh > 5f && state != AppState.Asset)
		{
			timerRefresh = 0f;
			if (Network.isClient || Network.isServer)
			{
				Get();
			}
		}
		*/
		//gestion des player objects : disparition en cas de chute
		if (timerGC > 5f)
		{
			timerGC = 0f;
			NA.GC();
		}

		foreach (NAObject o in listObjects) 
		{
			o.Process();
		}
		if (currentLocal != null)
			currentLocal.Process();

		NAServer.Process();


		//Mouse/Touch Raycasting
		RaycastHit hit;
		GameObject goPick = PickObject(Input.mousePosition, out hit);
		strPick = "";
		if (goPick)
		{
			AudioSource audio = goPick.GetComponent<AudioSource>();
			if (audio != null)
			{
				strPick += "AudioSource";
			}
		}

		//ACTION
		if (NAInput.GetControlDown(NAControl.Action))
		{
			NAToolBase t = tools[current_tool];
			t.Action();
		}

		//Previous tool
		if (NAInput.GetControlDown(NAControl.PreviousTool))
		{
			current_tool = (current_tool + tools.Length-1)%tools.Length;
			SetCurrentTool(tools[current_tool]);
		}

		//Next tool
		if (NAInput.GetControlDown(NAControl.NextTool))
		{
			current_tool = (current_tool + 1)%tools.Length;
			SetCurrentTool(tools[current_tool]);
        }

		//camera change
		if (NAInput.GetControlDown(NAControl.Camera))
		{
			current_camera = (current_camera + 1)%camerascripts.Length;
			SetCurrentCamera(camerascripts[current_camera]);
		}

		//touche menu
		if (NAInput.GetControlDown(NAControl.Menu))
		{
			bGUI = !bGUI;
			Cursor.visible = bGUI;
		}

		//à déplacer dans un tool ?
		if (Input.GetKeyDown(KeyCode.P))
		{
			//RaycastHit hit;
			//GameObject goPick = PickObject(Input.mousePosition, out hit);
			if (goPick != null && currentSelection != null)
			{
				currentSelection.go.transform.position = hit.point;
			}
		}
		/*
		if (Input.GetKeyDown(KeyCode.J))
		{
			GetComponent<NetworkView>().RPC("ServerSpawnObject", RPCMode.AllBuffered, "sphere", gameObject.transform.position+selectedCamera.transform.forward, Vector3.zero, colorAvatar);
		}
		if (Input.GetKeyDown(KeyCode.M))
		{
			GetComponent<NetworkView>().RPC("ServerSpawnObject", RPCMode.AllBuffered, "sphere", gameObject.transform.position+selectedCamera.transform.forward, selectedCamera.transform.forward, colorAvatar);
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			GetComponent<NetworkView>().RPC("ServerSpawnObject", RPCMode.AllBuffered, "cylinder", gameObject.transform.position+selectedCamera.transform.forward, selectedCamera.transform.forward, colorAvatar);
        }
        */
		/* removed on 30/11
		if (Input.GetKeyDown(KeyCode.Return))// || Input.GetButtonDown("Fire1"))
		{
			GetComponent<NetworkView>().RPC("ServerSpawnObject", RPCMode.AllBuffered, "cube", gameObject.transform.position+selectedCamera.transform.forward, selectedCamera.transform.forward, colorAvatar);
        }
		*/
		if (Input.GetKeyDown(KeyCode.T))// || Input.GetButtonDown("Fire3"))
		{
			GetComponent<NetworkView>().RPC("ServerSpawnObject", RPCMode.AllBuffered, "trunk", gameObject.transform.position+selectedCamera.transform.forward, selectedCamera.transform.forward, colorAvatar);
		}

		UnactivateCameras (); //FIXME

		/*
		foreach (WWW w in requests) 
		{
			if (w.isDone)
			{
				Debug.Log ("WWW is done : " + w.text);
				w.Dispose();
				requests.Remove(w);
			}
		}
		*/


		//force add
		if (Input.GetMouseButton(0))
		{
			if (goPick != null)
			{
				if (bPushObjects && Input.GetMouseButtonDown(0))
				{
					Vector3 force = (goPick.transform.position - selectedCamera.transform.position)/2f + (hit.normal * -1f )/2f;

					force.Normalize();
					if (goPick.GetComponent<Rigidbody>() != null)
					{
						goPick.GetComponent<Rigidbody>().AddForce(force*300f);
					}
				}
				if (bPullObjects && Input.GetMouseButtonDown(0))
				{
					Vector3 force = (goPick.transform.position - selectedCamera.transform.position)/2f + (hit.normal * -1f )/2f;
					force.Normalize();
					if (goPick.GetComponent<Rigidbody>() != null)
					{
                        goPick.GetComponent<Rigidbody>().AddForce(force*-300f);
                    }
                }
				if (bHit)
				{
					AudioSource source = goPick.GetComponent<AudioSource>();
					if (source)
					{
						source.loop = false;
						source.volume = 1f;
						source.Play();
					}
				}

				if (bGrab)
				{
					if (goGrab == null)
					{
						if (goPick)
						{
							goGrab = goPick;
						}
						PreviousMousePosition = Input.mousePosition;
						//GameObject goNew = GameObject.Instantiate(goPick, goAvatar.transform.position + goAvatar.transform.forward*2, Quaternion.identity) as GameObject;
						//ne fonctionne pas :
						//GameObject goNew = Network.Instantiate(goPick, goAvatar.transform.position + goAvatar.transform.forward*2, Quaternion.identity, 0) as GameObject;
						//goPick.transform.position = goAvatar.transform.position + goAvatar.transform.forward*2;
					}
					else
					{

					}
				}
				if (bFrotte)
				{
					AudioSource source = goPick.GetComponent<AudioSource>();
					if (source)
					{
						source.loop = true;
						source.volume += 0.05f;
						if (!source.isPlaying)
						{
                    		source.Play();
						}

						goFrotte = goPick;
					}
				}
			}
			if (goGrab != null)
			{
				Vector3 vMove = (Input.mousePosition-PreviousMousePosition);
				Vector3 pos = goGrab.transform.position;
				pos += Camera.main.transform.right*vMove.x*Time.deltaTime*0.1f;
				pos += Camera.main.transform.up*vMove.y*Time.deltaTime*0.1f;
                goGrab.transform.position = pos;
            }

		}
		else
		{
			if (goFrotte != null)
			{
				AudioSource source = goFrotte.GetComponent<AudioSource>();
				if (source)
                {
                    source.loop = false;
					source.volume -= 0.05f;
					//source.Stop();
				}
				//goFrotte = null;
            }

			if (goGrab)
			{
				Debug.Log("release grab");
				goGrab = null;
			}

        }
	}

	

	//Parse incoming server XML
	public void ParseXML(string str)
	{
		Debug.Log("parsing XML...");
		xml = new XmlDocument();
		xml.XmlResolver = null;
		try
		{
			xml.LoadXml(str);
		}
		catch (XmlException e)
		{
			string sextr = str.Substring(40422);
			Debug.Log (sextr);
			Debug.Log(e.Message);
		}
		xpn =  xml.CreateNavigator();

		XPathNodeIterator xpni_status = xpn.Select("/status");
		xpni_status.MoveNext();
		if (xpni_status.Current != null)
		{
			//xpni_status.MoveNext();
			string error = xpni_status.Current.GetAttribute("error","");
			if (error != "OK")
			{
				LogManager.LogError(error);
			}
			else
			{
				//last request succeded
				LogManager.Log("OK");
				if (state == AppState.Register)
					state = AppState.Login;
				else// if (state == AppState.Login || state == AppState.Space)
				{
					state = AppState.Spaces;
					NAServer.Get();
					//we tell the others that something changed
					if (NA.isServer() || NA.isClient())
					{
						GetComponent<NetworkView>().RPC("Refresh", RPCMode.Others);
					}
				}

			}
		}




		//

		XPathNodeIterator xpni_spaces = xpn.Select("/spaces");
		xpni_spaces.MoveNext();
		if (xpni_spaces.Current != null)
		{
			listSpaces.Clear();
			listAssets.Clear();
			XPathNodeIterator xpnic = xpni_spaces.Current.SelectChildren(XPathNodeType.Element);
			if (xpnic == null)
				return;
			while(xpnic.MoveNext())
			{
				if(xpnic.Current.Name.Equals("space"))
				{
					Space space = new Space();
					space.id 	= int.Parse(xpnic.Current.GetAttribute("id",""));
					space.name 	= xpnic.Current.GetAttribute("name","");
					space.type 	= xpnic.Current.GetAttribute("type","");
					space.creator 	= xpnic.Current.GetAttribute("creator","");

					//if (CurrentSpace != null)
					{
						//Debug.Log ("comparing space ID " + CurrentSpace.id + " AND " + space.id);
						//if (CurrentSpace.id == space.id)
						{
							//on instancie les objets
							XPathNodeIterator xpnicc = xpnic.Current.SelectChildren(XPathNodeType.Element);
							if (xpnicc == null)
								return;
							while(xpnicc.MoveNext())
							{
								if(xpnicc.Current.Name.Equals("object"))
								{
									space.objectCount ++;
									if (NA.CurrentSpace != null)
									{
										if (NA.CurrentSpace.id == space.id)
										{
											//Debug.Log ("Network load object");
											string name = xpnicc.Current.GetAttribute("name","");
											string filename = xpnicc.Current.GetAttribute("filename","");
											string id = xpnicc.Current.GetAttribute("id","");

											float x = float.Parse(xpnicc.Current.GetAttribute("x",""));
											float y = float.Parse(xpnicc.Current.GetAttribute("y",""));
											float z = float.Parse(xpnicc.Current.GetAttribute("z",""));

											float ax = float.Parse(xpnicc.Current.GetAttribute("ax",""));
											float ay = float.Parse(xpnicc.Current.GetAttribute("ay",""));
											float az = float.Parse(xpnicc.Current.GetAttribute("az",""));

											float sx = 1;
											float sy = 1;
											float sz = 1;

											try
											{
												sx = float.Parse(xpnicc.Current.GetAttribute("sx",""));
												sy = float.Parse(xpnicc.Current.GetAttribute("sy",""));
												sz = float.Parse(xpnicc.Current.GetAttribute("sz",""));
											}
											catch (System.Exception e)
											{

											}

											NetworkLoadObject(name, new Vector3 (x, y, z), new Vector3(ax, ay, az), new Vector3(sx, sy, sz), filename, id);
										}
										else
										{

										}
									}
									else
									{

									}
								}
							}
						}

					}
					listSpaces.Add (space);
				}
				if(xpnic.Current.Name.Equals("asset"))
				{
					Asset asset 	= new Asset();
					asset.id 		= int.Parse(xpnic.Current.GetAttribute("id",""));
					asset.name 		= xpnic.Current.GetAttribute("name","");
					asset.filename 	= xpnic.Current.GetAttribute("filename","");
					asset.creator 	= xpnic.Current.GetAttribute("creator","");
					asset.type 		= xpnic.Current.GetAttribute("type","");
					listAssets.Add (asset);
				}
			}
		}
	}



	void NetworkLoadObject(string _name, Vector3 _pos, Vector3 _angles, Vector3 _scale, string _filename, string _id)
	{
		GetComponent<NetworkView>().RPC("LoadObject", RPCMode.AllBuffered, _name, Network.AllocateViewID(), _pos, _angles, _scale, _filename, _id);
	}


	[RPC]
	void Refresh()
	{
		NAServer.Get();
	}


	[RPC]
	void LoadObject(string _name, NetworkViewID _viewID, Vector3 _pos, Vector3 _angles, Vector3 _scale, string _filename, string _id) 
	{
		//on regarde si l'object n'existe pas déjà
		foreach (NAObject o in listObjects) 
        {
			if (o.id == _id)
				return;
		}
		Debug.Log ("RPC LoadObject " + _name + " " + _filename);
		// créer un objet vide pour la synchro, puis ajouter l'objet téléchargé en child
		NAObject n = new NAObject (goRootSpace, _name, _pos, _angles, _scale, _filename, _viewID);
		n.id = _id;
		listObjects.Add(n);
		n.Download();
	}



	void Connect(string space)
	{
		Disconnect();
		NAServer.GetSpaceDescription(space);
		bStartPopup = false;
	}


	public void GoToSpace(int spaceid)
	{
		foreach (Space space in listSpaces)
		{
			if (space.id == spaceid)
			{
				GoToSpace(space);
				break;			
			}
		}
	}


	public void GoToSpace(Space space)
	{
		//fade black screen ?
		TransitionManager.Start(TransitionManager.FadeIn,3f,Color.white, null);

		NA.PreviousSpace = NA.CurrentSpace;
		NA.CurrentSpace = space;
		DestroyAllSpaceObjects();
		NAServer.Get(); //get the space description
	}
	


	public void DestroyAllSpaceObjects()
	{
		foreach (NAObject o in listObjects) 
		{
			if (o.go != null)
			{	
				GameObject.Destroy(o.go);
				o.go = null;
			}
		}
		listObjects.Clear ();
	}


	void Disconnect()
	{
		//bStartPopup = true;
		if (Network.isClient)
		{
			Network.RemoveRPCs(Network.player);
			Network.DestroyPlayerObjects(Network.player); 
			foreach (NetworkPlayer player in Network.connections)
			{
				Network.DestroyPlayerObjects(player); 
            }
		}
		DestroyAllSpaceObjects();
		selectedCamera = mainCamera;
        cameras.Clear ();
        cameras.Add (mainCamera.gameObject);
		//destroy local copies of Network objects
		NA.DestroyPlayerObjects2();
    }
    


    void OnGUI()
    {
		//reticule
		int reticule_size = 50;
		GUI.DrawTexture (new Rect (Screen.width/2-reticule_size/2, Screen.height/2, reticule_size, 2), texWhite);
		GUI.DrawTexture (new Rect (Screen.width/2, Screen.height/2-reticule_size/2, 2, reticule_size), texWhite);

		if (bDisplayAvatarNames)
		{
			List<GameObject> avatars = NA.GetAvatars();
			foreach (GameObject a in avatars)
			{
				if (Camera.main != null)
				{
					try
					{
						Vector3 pos2d = Camera.main.WorldToViewportPoint(a.transform.position);
						if (pos2d.z > 0)
						{
							GUI.color = Color.white;
							string strDisplay = a.name;
							pos2d.x = Mathf.Clamp(pos2d.x, -1,1);
							pos2d.y = Mathf.Clamp(pos2d.y, -1,1);
							int x = (int)(pos2d.x*Screen.width);
							int y = (int)(Screen.height-pos2d.y*Screen.height);
							GUI.Label (new Rect(x,y,100,30), strDisplay);
						}
					}
					catch (System.Exception e)
					{
						Debug.LogWarning("FIXME : avatars cleaning " + e.ToString());
					}
				}
			}
		}

		TransitionManager.DrawGUI();

		float loading = GetLoadingProgress();
		if (loading != -1f && loading != 1f)
		{
			GUI.color = Color.red;
			GUI.Label(new Rect(Screen.width/2-100, Screen.height/2-15, 200, 30), "loading... " + (int)(loading*100f) + "%");
			GUI.color = Color.white;
		}
		if (!bGUI)
		{
			return;
		}



		GUI.color = new Color (0, 0, 0, 0.5f);
		GUI.DrawTexture (new Rect (0, 0, Screen.width, 30), texWhite);
		GUI.color = Color.white;
		//GUI.Label(new Rect(0,0,400,30), "NewAtlantisNew Client - SAIC workshop");
		GUI.Label(new Rect(0,0,400,30), "New Atlantis Client v0.80");
		GUI.Label(new Rect(Screen.width-200, 0, 200, 30), strPick);




		//general loading bar
		float progress_val = 0;
		float progress_count = 0;
		foreach (NAObject o in listObjects) 
		{
			progress_count += 1f;
			/*if (o.www != null)
			{

				progress_val += o.www.progress;
			}
			else
			{
				progress_val += 1f;
			}
			*/
		}
		if (progress_count>0)
		{
			//Debug.Log ("val="+progress_val+"/" + progress_count);
			GUI.HorizontalScrollbar(new Rect(Screen.width-200, 0, 200, 30), 0, progress_val, 0, progress_count);
		}

		if (state == AppState.Login)
		{
			mGuiWinRectLogin = GUI.Window(1, mGuiWinRectLogin, WindowFunctionLogin, "Login");
			return;
		}
		else if (state == AppState.Register)
		{
			mGuiWinRectRegister = GUI.Window(11, mGuiWinRectRegister, WindowFunctionRegister, "Register");
			return;
		}
		if (state == AppState.Space)
		{
			mGuiWinRectSpace = GUI.Window(1, mGuiWinRectSpace, WindowFunctionSpace, "Space");
			//return;
		}
		if (state == AppState.Asset)
		{
			mGuiWinRectAsset = GUI.Window(12, mGuiWinRectAsset, WindowFunctionAsset, "Asset");
            return;
        }




		//bottom toolbar 
		int bottomy = Screen.height - 30;
		GUI.color = new Color (0, 0, 0, 0.5f);
		GUI.DrawTexture (new Rect (0, bottomy, Screen.width, 30), texWhite);
		GUI.color = Color.white;


		//tabs
		int tabx = 200;
		foreach (AppTab t in tabs)
		{
			GUI.color = t==tab?Color.red:Color.white;

			string caption = ""+t;
			if (t == AppTab.User)
				caption = "MyNA";
			if (GUI.Button (new Rect(tabx, 0, 80, 30), caption))
			{
				if (tab == t)
				{
					tab = AppTab.None;
				}
				else
				{
					tab = t;
				}
			}
			tabx += 80;
		}

        
        //to do : list of objects ?
        
        GUI.color = Color.white;

		switch (tab)
		{
		case AppTab.Chat:
			mGuiWinRectChat 	= GUI.Window(1, mGuiWinRectChat, WindowFunctionChat, "CHAT");
			break;
		case AppTab.About:
			mGuiWinRectAbout 	= GUI.Window(7, mGuiWinRectAbout, WindowFunctionAbout, "ABOUT");
			break;
		case AppTab.Options:
			mGuiWinRectOptions 	= GUI.Window(6, mGuiWinRectOptions, WindowFunctionOptions, "OPTIONS");
			break;
		case AppTab.Space:
			mGuiWinRectSpaces 	= GUI.Window(8, mGuiWinRectSpaces, WindowFunctionSpaces, "SPACE");
			break;
		case AppTab.Scene:
			mGuiWinRectScene = GUI.Window(3, mGuiWinRectScene, WindowFunctionScene, "SCENE");
			break;
		case AppTab.Lobby:
			GUI.Window(2, mGuiWinRectLobby, WindowFunctionLobby, "Lobby");
			break;
		case AppTab.User:
			GUI.Window(10, mGuiWinRectUser, WindowFunctionUser, "User");
			break;
		}
	}
















	public GameObject PickObject(Vector2 screenpos, out RaycastHit hit)
	{
		Vector3 v = screenpos;
		//RaycastHit hit;
		if (Camera.main != null)
		{
			Ray ray = Camera.main.ScreenPointToRay(v);
			if (Physics.Raycast(ray, out hit))
			{
				return hit.collider.gameObject;
        	}
		}
		hit = new RaycastHit();
        return null;
    }


	/*public void VerySpecialCase()
	{
		GameObject go = GameObject.Find ("Daylight Water");
		if (go)
		{
			go.AddComponent<AudioSource>();
			go.GetComponent<AudioSource>().clip = Resources.Load ("splash") as AudioClip;
			MeshCollider collider = go.AddComponent<MeshCollider>();
			collider.isTrigger = true;
			go.AddComponent<NAPlayOnTrigger>();
		}
	}
	*/
















	void OnConnectedToServer() 
	{
		Debug.Log("Connected to server");
		CreateNetworkAvatar();
		PlayEvent(2);
	}

	void OnPlayerConnected(NetworkPlayer player) 
	{
		//Called on the server whenever a player is disconnected from the server.
		Debug.Log("Player connected from " + player.ipAddress + ":" + player.port);
		ChatManager.Log("system", "player connected", 0);
		PlayEvent(3);
		LogManager.LogWarning("A new player just connected to the server.");

	}

	void OnPlayerDisconnected(NetworkPlayer player) 
	{
		//Called on the server whenever a player is disconnected from the server.
		PlayEvent(9);
		Debug.Log("Clean up after player " + player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player); //destroys the player objects including avatar
		LogManager.LogWarning("A new player just leaved the server.");
	}
    
	



    void OnDisconnectedFromServer(NetworkDisconnection info) 
	{
		LogManager.LogWarning("You have been disconnected from the server.");
		//Called on client during disconnection from server, but also on the server when the connection has disconnected.
		PlayEvent(10);
		if (Network.isServer)
			Debug.Log("Local server connection disconnected");
		else
			if (info == NetworkDisconnection.LostConnection)
				Debug.Log("Lost connection to the server");
		else
			Debug.Log("Successfully diconnected from the server");

		/*foreach (NetworkPlayer player in Network.connections)
        {
			if (Network.player.guid == player.guid)
			{
				Network.DestroyPlayerObjects(player); 
			}
		}
		Disconnect();
		*/

		if (Network.isClient)
		{
			Network.DestroyPlayerObjects(Network.player); 
		}

	}
	
    /*void CreateCube()
    {
        NetworkViewID viewID = Network.AllocateViewID();
		GetComponent<NetworkView>().RPC("SpawnBox", RPCMode.AllBuffered, viewID, transform.position);
	}
	*/

	//deprecated
	void NetworkConnectToSpace(string _space)
	{
		GetComponent<NetworkView>().RPC("ConnectToSpace", RPCMode.AllBuffered, _space);
	}

	void NetworkChat(string _message)
	{
		GetComponent<NetworkView>().RPC("Chat", RPCMode.AllBuffered, strName, _message);
	}
	

	[RPC]
	void SetColor(Color color) 
	{
		gameObject.GetComponent<MeshRenderer>().material.color = color;
    }
    
    [RPC]
	void Chat(string _name, string _message) 
	{
		ChatManager.Log(_name, _message, 0);
	}



	[RPC]
	void DestroyObject(NetworkViewID viewID)
	{
		NetworkView nv = NetworkView.Find (viewID);
		GameObject go = nv.gameObject;
		lock(NA.player_objects)
		{
			Debug.Log ("removing " + viewID);
			NA.player_objects.Remove(go);
		}
		GameObject.Destroy(go);
    }
    
    


    [RPC]
	void SpawnAvatar(NetworkViewID viewID, Vector3 color, string name) 
	{
		GameObject clone;
		//clone = GameObject.CreatePrimitive(PrimitiveType.Capsule);
		clone = GameObject.Instantiate(goPrefabAvatar, Vector3.zero, Quaternion.identity) as GameObject;

		clone.name = name;
		Collider.Destroy(clone.GetComponent<Collider>());
		NetworkView nView = clone.AddComponent<NetworkView>();
		nView.viewID = viewID;

        if (nView.owner == Network.player)
        {
			NA.goAvatar = clone;
        }
		else
		{
			NA.player_objects.Add(clone); //this is considered as a player object
		}

		MeshRenderer renderer = clone.GetComponent<MeshRenderer>();
		if (renderer != null)
		{
			renderer.material.color = new Color(color.x, color.y, color.z, 0.3f);
		}

		LogManager.Log ("New Avatar : " +  NAServer.strLogin);
		NA.AddAvatar(clone);
    }




    
    [RPC]
	void ConnectToSpace(string _space) 
	{
		Connect(_space);
	}














	void WindowFunctionChat (int windowID)
	{
		GUI.color = Color.white;
		//GUI.color = new Color(0.7f,0.7f,1f);
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Users : ");
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Player=" + Network.player.guid + " ip="+Network.player.ipAddress + " port=" + Network.player.port + " ping=" + Network.GetAveragePing(Network.player) + "ms");
		GUILayout.EndHorizontal();
        foreach (NetworkPlayer player in Network.connections)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Player="+player.guid + " ip="+player.ipAddress + " port=" + player.port + " ping=" + Network.GetAveragePing(player) + "ms");
			GUILayout.EndHorizontal();
        }
		GUILayout.Space(20);
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Chat : ");
		GUILayout.EndHorizontal();
		int maxcount = 12;
		int start = ChatManager.GetStart(maxcount);
		int end = ChatManager.GetEnd();
		for (int i=start;i<=end;++i)
		{
			GUILayout.BeginHorizontal();
			ChatEntry e = ChatManager.logs[i];
			GUILayout.Label ("[" + e.name + "] : " + e.str);
			GUILayout.EndHorizontal();
		}

		int diff = maxcount-(end-start);
		for (int i=0;i<diff;++i)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label ("");
			GUILayout.EndHorizontal();
		}

		GUILayout.BeginHorizontal();
		GUILayout.Label(strName, GUILayout.Width(80));
		strCurrentChatMessage = GUILayout.TextArea(strCurrentChatMessage, GUILayout.Width(120));
		if (GUILayout.Button("send", GUILayout.Width(80)))
		{
			NetworkChat(strCurrentChatMessage);
			strCurrentChatMessage = "";
		}
		GUILayout.EndHorizontal();
    }






	void WindowFunctionUser (int windowID)
	{
		GUI.color = Color.white;

		GUILayout.BeginHorizontal();
		GUILayout.Label ("Welcome, " + NAServer.strLogin + " !");
		if (GUILayout.Button ("Logout", GUILayout.Width(100)))
		{
			NAServer.strLogin = "";
			NAServer.strPassword = "";
			state = AppState.Login;
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label ("Assets");
		//GUILayout.EndHorizontal();

		/*
		GUILayout.BeginHorizontal();
		GUILayout.Label ("My Assets :");
		GUILayout.EndHorizontal();
		*/

		//GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Import a new asset to my library", GUILayout.Width(200 ))) 
		{
			CurrentAsset = null;
			strObjectName = "object_name";
			RefreshBundles();
			state = AppState.Asset;
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		foreach (TypeTab t in tabsAssets)
		{
			GUI.color = t==tabAssets?Color.red:Color.white;
			
			string caption = ""+t;
			if (t == TypeTab.Mine)
				caption = "My Assets";
			else if (t == TypeTab.SharedWithMe)
				caption = "Shared with me";
			if (GUILayout.Button (caption, GUILayout.Width(100)))
			{
				tabAssets = t;
            }
        }
		GUI.color = Color.white;
		AssetFilter = GUILayout.TextField (AssetFilter, GUILayout.Width(200));

        GUILayout.EndHorizontal();

		/*
		GUILayout.BeginHorizontal();
		GUILayout.Label("local file", GUILayout.Width(100));
		strFile = GUILayout.TextField (strFile, GUILayout.Width(200));
		GUILayout.Label("name", GUILayout.Width(100));
		strObjectName = GUILayout.TextField (strObjectName, GUILayout.Width(100));
		if (GUILayout.Button ("upload asset to my library"))
		{
			//read the bytes and post to the database...
			byte[] data = System.IO.File.ReadAllBytes(strFile);
			this.AssetAdd(data, strObjectName);
		}
		GUILayout.EndHorizontal();
		*/

		scrollPosMyAssets = GUILayout.BeginScrollView( scrollPosMyAssets, GUILayout.Height( 150 ) );

		foreach (Asset asset in listAssets)
		{
			if (
				(tabAssets == TypeTab.Mine && asset.creator == NAServer.strLogin || tabAssets == TypeTab.SharedWithMe && asset.type == "public" && asset.creator != NAServer.strLogin)
				&&
				(AssetFilter == "" || asset.name.Contains(AssetFilter) || asset.creator.Contains (AssetFilter))
			 )
            {

				GUILayout.BeginHorizontal();
				if (CurrentAsset == asset)
				{
					GUI.color = Color.red;
				}
				else
				{
					GUI.color = Color.white;
				}
				//GUILayout.Label(asset.name);
				if (GUILayout.Button(asset.name, GUILayout.Width(200)))
				{
					if (CurrentAsset == asset)
					{
						CurrentAsset = null;
					}
					else
					{
						CurrentAsset = asset;
					}
				}

				GUILayout.Label(asset.creator, GUILayout.Width(100));


				GUI.color = Color.white;




				GUILayout.EndHorizontal();
			}
		}
		

		GUILayout.EndScrollView();





		GUILayout.BeginHorizontal();
		if (CurrentAsset != null) //we act on selection
		{
			if (tabAssets == TypeTab.Mine && CurrentAsset.creator == NAServer.strLogin)
			{
				if (GUILayout.Button("update", GUILayout.Width(100)))
				{
					//CurrentAsset = asset;
					strObjectName 	= CurrentAsset.name;
					NAServer.bAssetPublic 	= CurrentAsset.type == "public" ? true : false;
					RefreshBundles();
					state = AppState.Asset;
				}
			}
			
			if (GUILayout.Button("add to space", GUILayout.Width(100)))
			{
				//we add this asset to the current space
				if (NA.CurrentSpace != null)
				{
					NAServer.ObjectAdd(NA.CurrentSpace, CurrentAsset);
				}
				else
				{
					LogManager.LogError("You are not connected to a space");
				}
			}
			
			GUI.color = Color.gray;
			if (GUILayout.Button("delete", GUILayout.Width(100)))
			{
			}
		}
		

		GUI.color = Color.white;
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("")){}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label ("Spaces");
		if (GUILayout.Button ("Create a new space", GUILayout.Width(150 ))) 
		{
			state = AppState.Space;
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		foreach (TypeTab t in tabsSpaces)
		{
			GUI.color = t==tabSpaces?Color.red:Color.white;
			
			string caption = ""+t;
			if (t == TypeTab.Mine)
				caption = "My Spaces";
			else if (t == TypeTab.SharedWithMe)
				caption = "Shared with me";
			if (GUILayout.Button (caption, GUILayout.Width(100)))
			{
				tabSpaces = t;
            }
        }
		GUI.color = Color.white;
		SpaceFilter = GUILayout.TextField (SpaceFilter, GUILayout.Width(200));
        GUILayout.EndHorizontal();
		GUISpacesHeader();
        scrollPosMySpaces = GUILayout.BeginScrollView( scrollPosMySpaces, GUILayout.Height( 150 ) );
		GUISpaces(true);
		GUILayout.EndScrollView();
		GUI.color = Color.white;
	}


	void GUISpacesHeader()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("name", 		GUILayout.Width(200));
		GUILayout.Label("label", 		GUILayout.Width(100));
		GUILayout.Label("ID", 			GUILayout.Width(50));
		GUILayout.Label("creator", 		GUILayout.Width(100));
		GUILayout.Label("creation date", GUILayout.Width(100));
		GUILayout.Label("last change", 	GUILayout.Width(100));
		GUILayout.Label("objects", 		GUILayout.Width(100));
		GUILayout.EndHorizontal();
	}


	void GUISpaces(bool userfilter)
	{
		foreach (Space space in listSpaces)
		{
			bool bShow = userfilter && (tabSpaces == TypeTab.Mine && space.creator == NAServer.strLogin || tabSpaces == TypeTab.SharedWithMe && space.type == "public" && space.creator != NAServer.strLogin && space.objectCount > 0);

			if ((bShow || !userfilter && space.objectCount > 0) && (SpaceFilter == "" || space.name.Contains(SpaceFilter) || space.creator.Contains (SpaceFilter)))
			{
				GUILayout.BeginHorizontal();
				if (space.name == strSpace)
				{
					GUI.color = Color.green;
				}
				else
				{
					GUI.color = Color.white;
				}
				if (GUILayout.Button(space.name, GUILayout.Width(200)))
				{
					
					strSpace = space.name;
					NA.CurrentSpace = space;
					Debug.Log ("Current Space id = " + NA.CurrentSpace.id);
					if (Network.isServer)
					{
						GoToSpace(space);
						ConnectToSpace(strSpace);
					}
					//Connect(space);
				}
				GUILayout.Label(space.type, GUILayout.Width(100));
				GUILayout.Label(""+space.id, GUILayout.Width(50));

				GUILayout.Label(space.creator, GUILayout.Width(100));
				GUILayout.Label("", GUILayout.Width(100)); //creation date?
				GUILayout.Label("", GUILayout.Width(100)); //last change?
				GUILayout.Label(""+space.objectCount, GUILayout.Width(100)); //share/invite?
				
				GUILayout.EndHorizontal();
			}
		}
	}
	void WindowFunctionLobby (int windowID)
	{
		GUI.color = Color.white;

		GUILayout.BeginHorizontal();
		GUILayout.Label ("Join an existing server...");
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label ("This machine ip : " + Network.player.ipAddress + "(" + Network.player.externalIP + ")" + " guid=" + Network.player.guid);// + " " + Network.player.externalIP);
		if (Network.isServer)
			GUILayout.Label ("[SERVER STARTED]");
		else if (Network.isClient)
			GUILayout.Label ("[CLIENT CONNECTED]");
		GUILayout.EndHorizontal();
	
		//GUILayout.BeginHorizontal();
			//if (GUILayout.Button ("start server at " + Network.player.ipAddress)) 
			/*if (GUILayout.Button ("start server", GUILayout.Width(200 ))) 
			{
				
				Network.InitializeServer(32, 7890, true);
				
				MasterServer.RegisterHost("NewAtlantis", strSpace, "comment...");

				CreateNetworkAvatar();
				//NetworkConnectToSpace(strSpace);
				if (strSpace != "")
					ConnectToSpace(strSpace);
				refreshHostList();
				//bNetwork = false;
				state = AppState.Game;
				//bSpace = false;
			}
			if (GUILayout.Button ("stop server", GUILayout.Width(200 ))) 
			{
				foreach (NetworkPlayer player in Network.connections)
				{
					if (Network.player.guid == player.guid)
					{
						Network.DestroyPlayerObjects(player); 
						Network.RemoveRPCs(player);
					}
				}
				DestroyPlayerObjects();
				Disconnect();
				Network.Disconnect();
				MasterServer.UnregisterHost();
				refreshHostList();
			}
			if (GUILayout.Button ("run standalone", GUILayout.Width(200 ))) 
			{
				ConnectToSpace(strSpace);
			}
			GUILayout.EndHorizontal();
*/
			//GUILayout.Space(20);
		//GUILayout.BeginArea(new Rect(0,0,1000,200), texWhite);
		GUILayout.BeginHorizontal();
		GUILayout.Label ("ACTIVE SESSIONS");
		if (GUILayout.Button ("Refresh", GUILayout.Width(100 ))) 
		{
			refreshHostList();
        }
        GUILayout.EndHorizontal();
		
		/*GUILayout.BeginHorizontal();
		strIP = GUILayout.TextField(strIP);
		
		
		if (GUILayout.Button ("connect to " + strIP)) 
		{
			Network.Connect(strIP, 7890);
		}
		GUILayout.EndHorizontal();
		*/
		//GUI.color = new Color(0.7f,0.7f,1f);
		
		
		/*foreach (NetworkPlayer player in Network.connections)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Player="+player.guid + " ip="+player.ipAddress + " port=" + player.port + " ping=" + Network.GetAveragePing(player) + "ms");
			GUILayout.EndHorizontal();
		}
		*/
		
		//serveurs dans le monde

		//texWhite
		scrollPos = GUILayout.BeginScrollView( scrollPos, GUILayout.Height( 150f ) );
		if( loading )
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label( "Loading..." );
			GUILayout.EndHorizontal();
		}
		else
		{
			//Debug.Log ("IP="+MasterServer.ipAddress);
			//Debug.Log ("PORT="+MasterServer.port);
			GUILayout.BeginHorizontal();
			GUILayout.Label( "name"	,GUILayout.Width(150 ));
			GUILayout.Label( "players"	,GUILayout.Width(50 ));
			GUILayout.Label( "IP/port"	,GUILayout.Width(140 ));
			GUILayout.Label( "GUID"	,GUILayout.Width(140 ));
			GUILayout.Label( "comment"	,GUILayout.Width(170 ));

			GUILayout.EndHorizontal();
			HostData[] hosts = MasterServer.PollHostList();
			for( int i = 0; i < hosts.Length; i++ )
			{
				HostData d = hosts[i];
				string ip = "";
				foreach (string s in d.ip)
				{
					ip += s + ".";
				}

				string lip = "";
				foreach (string s in d.ip)
				{
					lip += s;
					lip += ".";
				}
				string caption = d.gameName + "[" + d.connectedPlayers + "/" + d.playerLimit + "] on " +lip + ":" + d.port + " " + d.guid;




                if (currentHost != null)
				{
					if (currentHost.guid == d.guid)
					{
						GUI.color = Color.green;
					}
					else
					{
						GUI.color = Color.white;
					}
				}	
				else
				{
					GUI.color = Color.white;
                }
				GUILayout.BeginHorizontal();
				//GUILayout.Label( "Loading..." );
				if( GUILayout.Button( d.gameName,GUILayout.Width(150 )) )
				{
					//Network.Connect( hosts[i] );
					currentHost = d;
				}
				GUILayout.Label( "" + d.connectedPlayers + "/" + d.playerLimit	,GUILayout.Width(50 ));
				GUILayout.Label( lip+":"+d.port	,GUILayout.Width(140 ));
				GUILayout.Label( d.guid	,GUILayout.Width(140 ));
				GUILayout.Label( d.comment	,GUILayout.Width(170 ));
				GUILayout.EndHorizontal();
                
			}

			if( hosts.Length == 0 )
			{
				GUILayout.Label( "No servers running, you can start one below !" );
			}
			


		}
		GUILayout.EndScrollView();
		GUI.color = Color.white;
		GUILayout.BeginHorizontal();
		GUI.color = Network.isClient ? Color.gray : Color.white;
		if (GUILayout.Button ("Join server", GUILayout.Width(100 )) && !Network.isClient) 
		{
			Network.Connect(currentHost);
		}
		GUI.color = !Network.isClient ? Color.gray : Color.white;
		if (GUILayout.Button ("Leave server", GUILayout.Width(100 )) && Network.isClient) 
		{
			Disconnect();
			Network.Disconnect();
        }
		GUI.color = Color.white;


		/*if (GUILayout.Button ("Test open", GUILayout.Width(100 ))) 
		{
			//string strFile = EditorUtility.OpenFilePanel("open", "~", "unity3d");
			//Debug.Log ("file opened" + strFile);
		}
		*/

		//

		/*if (GUILayout.Button ("Close lobby ", GUILayout.Width(100 ))) 
		{
			state = AppState.Game;
			bNetwork = false;
		}
		*/

		/*if (GUILayout.Button ("Clean objects", GUILayout.Width(100 ))) 
		{
			//Network.RemoveRPCs(Network.player);
			//Network.DestroyPlayerObjects(Network.player); 
			//Application.LoadLevel(0);
			DestroyPlayerObjects();
		}
		*/



		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("")){}
		GUILayout.EndHorizontal();


		GUILayout.BeginHorizontal();
		GUILayout.Label ("SPACES LIBRARY");
		SpaceFilter = GUILayout.TextField (SpaceFilter, GUILayout.Width(200));
		GUILayout.EndHorizontal();

		GUISpacesHeader();
		scrollPosLobbySpaces = GUILayout.BeginScrollView( scrollPosLobbySpaces, GUILayout.Height( 150f ) );
		GUISpaces(false);
		GUILayout.EndScrollView();

		GUI.color = Color.white;
		GUILayout.BeginHorizontal();
		GUI.color = Color.gray;
		//if (GUILayout.Button ("start server at " + Network.player.ipAddress)) 
		if (GUILayout.Button ("run standalone", GUILayout.Width(120 ))) 
		{
			ConnectToSpace(strSpace);
		}
		if (GUILayout.Button ("stop standalone", GUILayout.Width(120 ))) 
		{
			//ConnectToSpace(strSpace);
			//TO DO !
		}

		GUI.color = Network.isServer ? Color.gray : Color.white;
		
		if (GUILayout.Button ("start server with selected space", GUILayout.Width(200 )) && !Network.isServer) 
		{
			TransitionManager.Start(TransitionManager.FadeIn,3f,Color.white, null);
			tab = AppTab.None; //hide windows
			Network.InitializeServer(32, 7890, true);
			string strGameName = strSpace + " [" + NAServer.strLogin + "]";
			MasterServer.RegisterHost("NewAtlantis", strGameName, "created : " + System.DateTime.Now + " on " + SystemInfo.deviceModel + " running " + SystemInfo.operatingSystem);
			CreateNetworkAvatar();
			//if (strSpace != "")
			//	ConnectToSpace(strSpace);
			NAServer.Get(); //le Get avec un selected space forcera la création des objets : à revoir...

			refreshHostList();
			//bNetwork = false;
			state = AppState.Game;
			//bSpace = false;
		}
		GUI.color = !Network.isServer ? Color.gray : Color.white;
		if (GUILayout.Button ("stop server", GUILayout.Width(200 )) && Network.isServer) 
		{
			foreach (NetworkPlayer player in Network.connections)
			{
				//if (Network.player.guid == player.guid)
				{
					Network.DestroyPlayerObjects(player); 
					Network.RemoveRPCs(player);
				}
			}
			NA.DestroyPlayerObjects2();
			Disconnect();
			Network.Disconnect();
			MasterServer.UnregisterHost();
			refreshHostList();
		}

		GUILayout.EndHorizontal();
			
		GUILayout.Space(20);
		//GUILayout.EndArea();
		GUI.color = Color.white;
			

	}
    


	void WindowFunctionScene (int windowID)
	{
		GUI.color = Color.white;

		GUILayout.BeginHorizontal();
		GUILayout.Label ("Cameras " + "[" + cameras.Count + "]");
		GUILayout.EndHorizontal();
		scrollPosCameras = GUILayout.BeginScrollView( scrollPosCameras, GUILayout.Height( 60 ) );
		foreach (GameObject c in cameras)
		{
			if (selectedCamera == c.GetComponent<Camera>())
				GUI.color = Color.red;
			else
				GUI.color = Color.white;
			string name = c.name;
			if (c.gameObject.transform.parent != null)
				name = c.gameObject.transform.parent.gameObject.name;
			GUILayout.BeginHorizontal();
			if (GUILayout.Button (name, GUILayout.Width(150)))
			{
				selectedCamera.enabled = false;
				selectedCamera.GetComponent<AudioListener>().enabled = false;
				selectedCamera = c.GetComponent<Camera>();
				selectedCamera.enabled = true;
				selectedCamera.GetComponent<AudioListener>().enabled = true;
			}
			string path = getPath(c.gameObject, "");
			GUILayout.Label (path, GUILayout.Width(380));
            
            GUILayout.EndHorizontal();
			GUI.color = Color.white;
		}
		GUILayout.EndScrollView();

		GUI.color = Color.white;
		Light[] lights = Light.FindObjectsOfType (typeof(Light)) as Light[];
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Lights " + "[" + lights.Length + "]");
		GUILayout.EndHorizontal();
		scrollPosLights = GUILayout.BeginScrollView( scrollPosLights, GUILayout.Height( 180 ) );
		foreach (Light l in lights)
		{
			GUI.color = l.enabled ? Color.red : Color.white;
			if (l.name.Contains("Creature"))
				continue;
			if (l.name.Contains("Area"))
				continue;
			GUILayout.BeginHorizontal();
			if (GUILayout.Button (l.name, GUILayout.Width(150)))
			{
				l.enabled = !l.enabled;
			}
			string path = getPath(l.gameObject, "");
			GUILayout.Label (path, GUILayout.Width(380));

			string ltype = "" + l.type;
			GUILayout.Label (ltype, GUILayout.Width(100));
			GUILayout.Label (""+(int)l.range, GUILayout.Width(100));
            GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
		GUI.color = Color.white;
		AudioSource[] sources = AudioSource.FindObjectsOfType (typeof(AudioSource)) as AudioSource[];
		GUILayout.BeginHorizontal();
		GUILayout.Label ("AudioSources " + "[" + sources.Length + "]");
		GUILayout.EndHorizontal();
		scrollPosSources = GUILayout.BeginScrollView( scrollPosSources, GUILayout.Height( 180 ) );
		foreach (AudioSource s in sources)
		{
			GUI.color = s.enabled ? Color.red : Color.white;
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button (s.name, GUILayout.Width(150)))
			{
				
				s.enabled = !s.enabled;
			}
			GUI.color = Color.white;

			string path = getPath(s.gameObject, "");
			GUILayout.Label (path, GUILayout.Width(380));

			s.volume = GUILayout.HorizontalSlider(s.volume, 0, 1, GUILayout.Width(50));
			GUILayout.Label (""+s.spatialBlend, GUILayout.Width(20));
			GUILayout.Label (""+s.rolloffMode, GUILayout.Width(70));
			GUILayout.Label (""+(int)s.maxDistance, GUILayout.Width(50));

			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();


	}
    

    


	//=====================================
	//register window
	//=====================================
	void WindowFunctionRegister (int windowID)
	{
		GUI.color = Color.white;
		GUILayout.BeginHorizontal();
		GUILayout.Label("Welcome to the New Atlantis. Here you can register as a new New Atlantis user...");
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Login", GUILayout.Width(100));
		NAServer.strLogin = GUILayout.TextField (NAServer.strLogin);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Password", GUILayout.Width(100));
		NAServer.strPassword = GUILayout.PasswordField (NAServer.strPassword, "*"[0]);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Retype password", GUILayout.Width(100));
		NAServer.strPasswordRetype = GUILayout.PasswordField (NAServer.strPasswordRetype, "*"[0]);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Email", GUILayout.Width(100));
		NAServer.strEmail = GUILayout.TextField (NAServer.strEmail);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Cancel"))
		{
			state = AppState.Login;
			return;
		}
		if (GUILayout.Button ("Register"))
		{
			if (NAServer.strPassword != NAServer.strPasswordRetype)
			{
				LogManager.LogError("ERROR, password must match!");
			}
			else if (!NAServer.strEmail.Contains("@") || !NAServer.strEmail.Contains("."))
			{
				LogManager.LogError("ERROR, please enter a valid email!");
			}
			else if (NAServer.strPassword.Length < 8)
			{
				LogManager.LogError("ERROR, your password must be at least 8 characters long");
			}
			else
			{
				NAServer.UserRegister();
				
			}

			return;
		}
		GUILayout.EndHorizontal();
		//GUI.DragWindow();
	}



	//=====================================
	//Asset window : create or modify an asset
	//=====================================
	void WindowFunctionAsset (int windowID)
	{
		GUI.color = Color.white;
		GUILayout.BeginHorizontal();
		if (CurrentAsset != null)
		{
			GUILayout.Label("Modify asset below");
		}
		else
		{
			GUILayout.Label("Create a new asset below");
		}
		GUILayout.EndHorizontal();

		if (info != null)
		{
			scrollPosAssetFileDialog = GUILayout.BeginScrollView( scrollPosAssetFileDialog, GUILayout.Height( 200 ) );
			foreach (FileInfo f in info) 
			{
				GUILayout.BeginHorizontal();
				GUI.color = strFile == f.Name ? Color.red : Color.white;
				if (GUILayout.Button(f.Name, GUILayout.Width(300)))
				{
					strFile = f.Name;
				}
            	GUILayout.EndHorizontal();
        	}
        	GUILayout.EndScrollView();
		}
		else
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Please create a <Bundles> directory in the app directory or in the Unity project (besides Assets)! If this still does not work, check that you are in standalone target in the build settings.");
			GUILayout.EndHorizontal();
		}

		GUI.color = Color.white;

		GUILayout.BeginHorizontal();
		GUILayout.Label("local file", GUILayout.Width(70));
		strFile = GUILayout.TextField (strFile, GUILayout.Width(300));
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("name", GUILayout.Width(70));
		strObjectName = GUILayout.TextField (strObjectName, GUILayout.Width(100));

		GUILayout.Label("Type", GUILayout.Width(100));
		NAServer.bAssetPublic = GUILayout.Toggle(NAServer.bAssetPublic, "public");
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Cancel"))
		{
			state = AppState.Spaces;
            return;
        }
#if UNITY_WEBPLAYER
		//this feature is not available on the Web Player because of disk access restriction
#else
		if (GUILayout.Button ("upload asset to my library"))
		{
			if (strObjectName != "object_name")
			{
				//read the bytes and post to the database...
				byte[] data = System.IO.File.ReadAllBytes("Bundles/"+strFile);
				if (CurrentAsset != null)
				{
					NAServer.AssetUpdate (CurrentAsset.id, data, strObjectName);
				}
				else
				{
					NAServer.AssetAdd(data, strObjectName);
				}
			}
			else
			{
				LogManager.LogError("ERROR, please provide a name for your object !");
			}
        }
#endif
        GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if (NAServer.wwwPost != null)
		{
			float p = NAServer.wwwPost.uploadProgress;
			//GUILayout.Label("Upload=" + p, GUILayout.Width(100));
			GUILayout.HorizontalScrollbar(0, p, 0, 1);
		}
		GUILayout.EndHorizontal();
	}



	//=====================================
	//Space window
	//=====================================
	void WindowFunctionSpace (int windowID)
	{
		GUI.color = Color.white;
		GUILayout.BeginHorizontal();
		GUILayout.Label("Create or Modify space below");
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Name", GUILayout.Width(100));
		NAServer.strSpaceName = GUILayout.TextField (NAServer.strSpaceName);
		GUILayout.EndHorizontal();
		
		/*GUILayout.BeginHorizontal();
		GUILayout.Label("Type", GUILayout.Width(100));
		strSpaceType = GUILayout.TextField (strSpaceType);
		GUILayout.EndHorizontal();*/

		GUILayout.BeginHorizontal();
		GUILayout.Label("Type", GUILayout.Width(100));
		NAServer.bSpacePublic = GUILayout.Toggle(NAServer.bSpacePublic, "public");
		GUILayout.EndHorizontal();


		
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Cancel"))
		{
			state = AppState.Spaces;
			return;
		}
		if (GUILayout.Button ("Delete"))
		{
			//SpaceDelete();
			return;
		}
		if (GUILayout.Button ("Create"))
		{
			NAServer.SpaceCreate();
			return;
		}
		GUILayout.EndHorizontal();
	}

    //=====================================
    //login window
    //=====================================
	void WindowFunctionLogin (int windowID)
	{
		GUI.color = Color.white;
		GUILayout.BeginHorizontal();
		GUILayout.Label("Welcome to the New Atlantis. New Atlantis is a shared (multi-user) online virtual world dedicated to audio experimentation and practice. Unlike most online worlds where image is the primary concern, in New Atlantis sound comes first.");
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Login", GUILayout.Width(100));
		NAServer.strLogin = GUILayout.TextField (NAServer.strLogin);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Password", GUILayout.Width(100));
		NAServer.strPassword = GUILayout.PasswordField (NAServer.strPassword, "*"[0]);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Register..."))
		{
			state = AppState.Register;
			return;
		}
		if (GUILayout.Button ("Connect"))
		{
			NAServer.UserConnect();
			return;
		}
		GUILayout.EndHorizontal();
		//GUI.DragWindow();
	}









	void WindowFunctionSpaces (int windowID)
	{
		GUI.color = Color.white;

		GUILayout.BeginHorizontal();
		GUILayout.Label("OBJECTS IN SPACE", GUILayout.Width(200));
		GUILayout.Label("Click on an object to modify it.", GUILayout.Width(200));
		GUILayout.EndHorizontal();

		scrollPosSpace = GUILayout.BeginScrollView( scrollPosSpace, GUILayout.Height( 400 ) );

        //HERE
        foreach (NAObject o in listObjects) 
		{
			if (o == currentSelection)
			{
				GUI.color = Color.red;
			}
			else
			{
				GUI.color = Color.white;
			}

			GUILayout.BeginHorizontal();
			if (o.go != null)
			{
				if (GUILayout.Button (o.name, GUILayout.Width(150))) //50
				{
					//Alex Gizmo
					GameObject goGizmo = GameObject.Find("TRS Gizmo");
					TRS_Gizmo trs = goGizmo.GetComponent<TRS_Gizmo>();

					if (currentSelection != null)
						currentSelection.goGizmo.SetActive(false);
					
					if (currentSelection == o)
					{
						currentSelection = null;
						NA.PlayPhysics();
					}
					else
					{
						currentSelection = o;
						currentSelection.goGizmo.SetActive(true);
						trs.startTRSGizmo(currentSelection.go);
						NA.PausePhysics();
					}
					
				
					if (currentSelection != null)
					{
					}
					else
					{
						trs.stopTRSGizmo();
					}
				}
			}
			else
				GUILayout.Label(o.name, GUILayout.Width(200));

			int distance = (int)(Camera.main.transform.position-o.go.transform.position).magnitude;
			string strLabel = "" + distance + /*o.file + */"m [" + (int)o.go.transform.position.x + ";" +(int)o.go.transform.position.y + ";" + (int)o.go.transform.position.z + "]"; 
			GUILayout.Label(strLabel, GUILayout.Width(100));
            
            //if (o.www != null)
			if (o.downloading)
			{
				//HERE
				//GUILayout.Label ("downloading " + o.www.progress, GUILayout.Width(100));
				GUILayout.HorizontalScrollbar(0, 0.5f/*o.www.progress*/, 0, 1, GUILayout.Width(50));
			}
			else
				//GUILayout.Label (""+o.downloaded/1000 + " KB", GUILayout.Width(200));
				GUILayout.Label ("OK", GUILayout.Width(50));
			//GUI.Label(new Rect(400,y,100,30), o.GetStatus());

			GUILayout.Label(""+o.GetAudioSourcesCount(), GUILayout.Width(100));
			GUILayout.Label(o.file, GUILayout.Width(300));
			GUILayout.EndHorizontal();

		}
		GUILayout.EndScrollView();
		GUI.color = Color.white;
		GUILayout.BeginHorizontal();
		if (currentSelection != null)
		{
			if (GUILayout.Button ("revert", GUILayout.Width(100)))
			{
				//retrieve from NAObject
				currentSelection.go.transform.position  	= currentSelection.position;
				currentSelection.go.transform.eulerAngles  	= currentSelection.angles;
				currentSelection.go.transform.localScale  	= currentSelection.scale;
			}
			if (GUILayout.Button ("save", GUILayout.Width(100)))
			{
				//save to NAObject and send to database
				currentSelection.position 	= currentSelection.go.transform.position;
				currentSelection.angles 	= currentSelection.go.transform.eulerAngles;
				currentSelection.scale 		= currentSelection.go.transform.localScale;
				NAServer.ObjectUpdate(currentSelection.id, currentSelection.position, currentSelection.angles, currentSelection.scale);
			}

			if (GUILayout.Button ("delete", GUILayout.Width(100)))
			{
				currentSelection.position = currentSelection.go.transform.position;
				currentSelection.angles = currentSelection.go.transform.eulerAngles;

				NAServer.ObjectDelete(currentSelection.id);
				GameObject.Destroy(currentSelection.go);
				currentSelection.go = null;
				listObjects.Remove(currentSelection);
				currentSelection = null;
				GUILayout.EndHorizontal();
				return;
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button ("x-"))
			{
				currentSelection.go.transform.position += new Vector3(-1,0,0);
			}
			if (GUILayout.Button ("x+"))
			{
				currentSelection.go.transform.position += new Vector3(1,0,0);
			}
			if (GUILayout.Button ("y-"))
			{
				currentSelection.go.transform.position += new Vector3(0,-1,0);
			}
			if (GUILayout.Button ("y+"))
			{
				currentSelection.go.transform.position += new Vector3(0,1,0);
			}
			if (GUILayout.Button ("z-"))
			{
				currentSelection.go.transform.position += new Vector3(0,0,-1);
			}
			if (GUILayout.Button ("z+"))
			{
				currentSelection.go.transform.position += new Vector3(0,0,1);
			}
			
			if (GUILayout.Button ("ry+"))
			{
				currentSelection.go.transform.eulerAngles += new Vector3(0,10,0);
			}
			if (GUILayout.Button ("ry-"))
			{
				currentSelection.go.transform.eulerAngles += new Vector3(0,-10,0);
			}
		}
		GUILayout.EndHorizontal();
    }




	void WindowFunctionAbout (int windowID)
	{
		GUI.color = Color.white;
		GUILayout.BeginHorizontal();
		GUILayout.TextArea("New Atlantis is a shared (multi-user) online virtual world dedicated to audio experimentation and practice. Unlike most online worlds where image is the primary concern, in New Atlantis sound comes first.");
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Mouse drag : look");
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Arrow keys : move");
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Space : clap");
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Return : Throw cube");
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("T : Create trunk");
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("S : Create sphere");
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("P : Put selection at mouse pos");
		GUILayout.EndHorizontal();
        GUI.DragWindow();
    }


	void WindowFunctionOptions (int windowID)
	{
		GUI.color = Color.white;

		GUI.color = NAAudioSource.bDisplayAudioSourceName ? Color.red : Color.white;
		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Display AudioSource names"))
		{
			NAAudioSource.bDisplayAudioSourceName = !NAAudioSource.bDisplayAudioSourceName;
		}
		GUILayout.EndHorizontal();

		GUI.color = NA.bAugmentAudioSources ? Color.red : Color.white;
		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Augment AudioSources"))
		{
			NA.bAugmentAudioSources = !NA.bAugmentAudioSources;
		}
		GUILayout.EndHorizontal();

		GUI.color = bPushObjects ? Color.red : Color.white;
		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("push objects on click"))
		{
			bPushObjects = !bPushObjects;
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("master vol", GUILayout.Width(100));
		AudioListener.volume = GUILayout.HorizontalSlider(AudioListener.volume, 0, 5);
		GUILayout.EndHorizontal();

		GUI.DragWindow();
    }
    

	void refreshHostList()
	{
		// let the user know we are awaiting results from the master server
		loading = true;
		MasterServer.ClearHostList();
		MasterServer.RequestHostList( "NewAtlantis" );
	}


	// this is called when the Master Server reports an event to the client – for example, server registered successfully, host list received, etc
	void OnMasterServerEvent( MasterServerEvent msevent )
	{
		LogManager.Log("MasterServer Event : " + msevent);
		if( msevent == MasterServerEvent.HostListReceived )
		{
			// received the host list, no longer awaiting results
			loading = false;
		}
		PlayEvent(1);
	}


	void PlayEvent(int ev)
	{
		AudioClip clip = null;
		if (ev < 10)
			clip = Resources.Load ("MIP0" + ev) as AudioClip;
		else
			clip = Resources.Load ("MIP" + ev) as AudioClip;
		AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
	}
    //
    
	void OnNetworkInstantiate(NetworkMessageInfo info) 
	{
		//info.sender.guid
		//Debug.Log(info.);
		Debug.Log("instantiate");
	}
     

    

	public  void RefreshBundles()
	{
		try
		{
#if UNITY_WEBPLAYER
#else
			DirectoryInfo dir = new DirectoryInfo("Bundles");
			info = dir.GetFiles("*.unity3d");
#endif
		}
		catch (System.Exception e)
        {
            
        }
    }


	public string getPath(GameObject go, string path)
	{
		if (go.transform.parent != null)
		{
			path += getPath(go.transform.parent.gameObject, path);
		}

		return path+"/"+go.name;//+"/"+go.name;
	}

    
    
}
