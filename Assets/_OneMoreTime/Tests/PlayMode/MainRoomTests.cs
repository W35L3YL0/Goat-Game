#if UNITY_EDITOR
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
namespace OneMoreTime.Tests {
public class MainRoomTests {
 MainRoom room;
 void Step(float move=0,bool jump=false){room.Step(new InputFrame{move=move,jump=jump,held=true},.02f);Physics2D.SyncTransforms();}
 void Walk(float x){for(int i=0;i<600&&Mathf.Abs(room.player.Position.x-x)>.08f;i++)Step(Mathf.Sign(x-room.player.Position.x));for(int i=0;i<8;i++)Step();Assert.That(room.player.Position.x,Is.EqualTo(x).Within(.3f));}
 void Jump(float x,float y){
  Step(0,true);
  for(int i=0;i<160;i++){
   float dx=x-room.player.Position.x;
   // One-way grass permits approach from below; solid ledges still block sides.
   float move=Mathf.Abs(dx)>.06f?Mathf.Sign(dx):0;
   Step(move);
   if(room.player.Grounded&&i>5)break;
  }
  Assert.That(room.player.Position.y,Is.EqualTo(y).Within(.04f),"Landing at "+x+", "+y+"; actual "+room.player.Position);
 }
 [UnityTest]public IEnumerator RoomOneRouteCheckpointPitAndExit(){
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/"+MainRoom.SceneName+".unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  room=Object.FindFirstObjectByType<MainRoom>();room.AutoTick=false;room.SendMessage("LateUpdate");
  Assert.That(room.view.orthographicSize*2*room.view.aspect,Is.EqualTo(20).Within(.01f));
  for(int i=0;i<160&&room.Phase==MainRoom.RoomPhase.Entering;i++)Step();
  Assert.AreEqual(MainRoom.RoomPhase.Playing,room.Phase);Assert.That(room.Prompt,Does.Contain("move"));
  Walk(20);Assert.That(room.Prompt,Does.Contain("jump"));Walk(24.2f);Jump(26,2.8f);Walk(30.8f);
  Assert.IsTrue(room.CheckpointSaved);Assert.Greater(room.statueEyes[0].color.g,.9f);
  Walk(35.8f);for(int i=0;i<100&&room.Deaths==0;i++)Step();
  Assert.AreEqual(1,room.Deaths);Assert.That(room.player.Position.x,Is.EqualTo(30.8f).Within(.05f));Assert.IsTrue(room.CheckpointSaved);
  Walk(34.2f);Jump(39.5f,2.8f);Walk(60);Jump(60,5.3f);Walk(58.8f);Jump(57,7.3f);Walk(57.3f);Jump(59.7f,9.8f);Walk(62.9f);Jump(65,12.4f);
  for(int i=0;i<300&&room.Phase==MainRoom.RoomPhase.Playing;i++)Step(1);
  Assert.AreEqual(MainRoom.RoomPhase.Exiting,room.Phase);
  for(int i=0;i<100&&room.Phase==MainRoom.RoomPhase.Exiting;i++)Step(-1,true);
  Assert.AreEqual(MainRoom.RoomPhase.Complete,room.Phase);Assert.AreEqual(1,room.Fade);
  Assert.Greater(room.player.Position.x,room.roomBounds.xMax);
 }
}
}
#endif
