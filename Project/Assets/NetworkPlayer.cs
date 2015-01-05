using System;
using UnityEngine;

public class NetworkPlayer{

	private string id;
	private GameObject avatar;
	private Vector3 position = new Vector3(0,0,0);
	private Quaternion rotation = new Quaternion(0,0,0,0);
	private float shoot;
	private bool isMessage = false;
	private int messageFrame = 0;
	private string chatMessage = "";
	private string message = "";

	public NetworkPlayer (){
	}

	public NetworkPlayer (string id){
		this.id = id;
		this.avatar = GameObject.CreatePrimitive(PrimitiveType.Cube);
		this.position = new Vector3(732f,1.5f,500f);
	}

	public NetworkPlayer (string id, GameObject playerAvatar, Vector3 position){
		this.id = id;
		this.avatar = playerAvatar;
		this.position = position;
	}

	public void update(){
		this.Avatar.transform.position = this.position;
		this.Avatar.transform.rotation = this.rotation;
	}

	public string toJson(){
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.Append("{\"id\":\""+this.id+"\",");
		sb.Append("\"x\":\""+this.position.x+"\",");
		sb.Append("\"y\":\""+this.position.y+"\",");
		sb.Append("\"z\":\""+this.position.z+"\",");
		sb.Append("\"rx\":\""+this.rotation.x+"\",");
		sb.Append("\"ry\":\""+this.rotation.y+"\",");
		sb.Append("\"rz\":\""+this.rotation.z+"\",");
		sb.Append("\"rw\":\""+this.rotation.w+"\",");
		sb.Append("\"s\":\""+this.shoot+"\",");
		sb.Append("\"m\":\""+this.message+"\"}");
		return sb.ToString();
	}

	public string Id {
		get {
			return this.id;
		}
	}

	public Vector3 Position {
		get {
			return this.position;
		}
		set {
			position = value;
		}
	}

	public GameObject Avatar {
		get {
			return this.avatar;
		}
		set {
			avatar = value;
		}
	}

	public Quaternion Rotation {
		get {
			return this.rotation;
		}
		set {
			rotation = value;
		}
	}

	public float Shoot {
		get {
			return this.shoot;
		}
		set {
			shoot = value;
		}
	}

	public bool IsMessage {
		get {
			return this.isMessage;
		}
		set {
			isMessage = value;
		}
	}

	public int MessageFrame {
		get {
			return this.messageFrame;
		}
		set {
			messageFrame = value;
		}
	}

	public string ChatMessage {
		get {
			return this.chatMessage;
		}
		set {
			chatMessage = value;
		}
	}

	public string Message {
		get {
			return this.message;
		}
		set {
			message = value;
		}
	}
}


