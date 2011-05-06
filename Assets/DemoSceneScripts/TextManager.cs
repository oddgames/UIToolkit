using UnityEngine;
using System.Collections;


public class TextManager : MonoBehaviour
{
	private UITextInstance text1;
	private UITextInstance text2;
	private UITextInstance text3;
	private UITextInstance text4;
	

	void Start()
	{
		// setup our text instance which will parse our .fnt file and allow us to
		var text = new UIText( "prototype", "prototype.png" );

		// spawn new text instances showing off the relative positioning features by placing one text instance in each corner
		var x = UIRelative.xPercentFrom( UIxAnchor.Left, 0f );
		var y = UIRelative.yPercentFrom( UIyAnchor.Top, 0f );
		text1 = text.addTextInstance( "hello man.  I have a line\nbreak", x, y );
		
		
		var textSize = text.sizeForText( "testing small bitmap fonts", 0.3f );
		x = UIRelative.xPercentFrom( UIxAnchor.Left, 0f );
		y = UIRelative.yPercentFrom( UIyAnchor.Bottom, 0f, textSize.y );
		text2 = text.addTextInstance( "testing small bitmap fonts", x, y, 0.3f );
		
		
		textSize = text.sizeForText( "with only\n1 draw call\nfor everything", 0.5f );
		x = UIRelative.xPercentFrom( UIxAnchor.Right, 0f, textSize.x );
		y = UIRelative.yPercentFrom( UIyAnchor.Top, 0f );
		text3 = text.addTextInstance( "with only\n1 draw call\nfor everything", x, y, 0.5f, 5, Color.yellow );
		
		
		textSize = text.sizeForText( "kudos" );
		x = UIRelative.xPercentFrom( UIxAnchor.Right, 0f, textSize.x );
		y = UIRelative.yPercentFrom( UIyAnchor.Bottom, 0f, textSize.y );
		text4 = text.addTextInstance( "kudos", x, y );
		
		
		textSize = text.sizeForText( "Be sure to try this with\niPhone and iPad resolutions" );
		var center = UIRelative.center( textSize.x, textSize.y );
		text.addTextInstance( "Be sure to try this with\niPhone and iPad resolutions", center.x, center.y, 1f, 4, Color.red );
		
		StartCoroutine( waitThenRemoveText() );
	}
	

	IEnumerator waitThenRemoveText()
	{
		yield return new WaitForSeconds( 2.0f );
		text1.clear();
		
		yield return new WaitForSeconds( 1.0f );
		text2.text = "gonna change the little text";
		text2.setColorForAllLetters( Color.green );
		
		yield return new WaitForSeconds( 2.0f );
		text3.clear();
		
		yield return new WaitForSeconds( 1.0f );
		text4.clear();
	}

}
