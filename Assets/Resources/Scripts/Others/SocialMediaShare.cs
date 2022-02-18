using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SocialMediaShare : MonoBehaviour
{
	[SerializeField] GameObject Panel_share;


	public void ShareScore()
	{
		//open the score panel
		//Panel_share.SetActive(true);//show the panel
		StartCoroutine("TakeScreenShotAndShare");
	}

	IEnumerator TakeScreenShotAndShare()
	{
		yield return new WaitForEndOfFrame();

		Texture2D tx = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		tx.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		tx.Apply();

		string path = Path.Combine(Application.temporaryCachePath, "BTSI_Highscore_Screenshot.png");//image name
		File.WriteAllBytes(path, tx.EncodeToPNG());

		Destroy(tx); //to avoid memory leaks

		new NativeShare()
			.AddFile(path)
			.SetSubject("Be the SPACE INVADERS - Highscore")
			.SetText("My highscore in the fight against the Terrans. Can you beat me? " +
					 "\nBe the SPACE INVADERS on Google Play:" +
					 "\nhttps://play.google.com/store/apps/details?id=com.WRB.Studio.BetheSpaceInvaders")
			.Share();


		//Panel_share.SetActive(false); //hide the panel
	}
}
