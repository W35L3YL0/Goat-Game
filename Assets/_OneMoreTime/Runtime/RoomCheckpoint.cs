using UnityEngine;
namespace OneMoreTime {
public sealed class RoomCheckpoint:MonoBehaviour {
 public SpriteRenderer[] eyes;public bool Saved{get;private set;}
 public void Save(){Saved=true;foreach(var eye in eyes)if(eye!=null)eye.color=new Color(.25f,1,.35f);}
}
}
