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
		var text = new UIText( "prototype.fnt", "prototype.png" );
		
		var vec1 = UI.relativeVec2( .3f, UIxRelativeTo.Right, .1f, UIyRelativeTo.Top );
		var vec2 = new Vector2( 10, 180 );
		var vec3 = new Vector2( 10, 220 );
		var vec4 = new Vector2( 100, 150 );
		
		// add 4 text instances
		text1 = text.addTextInstance( "hello\nman", vec1, 1.0f );
		text2 = text.addTextInstance( "testing small bitmap fonts", vec2, 0.3f, 2, Color.white );
		text3 = text.addTextInstance( "with only 1 draw call for everything", vec3, 0.7f, 5, Color.yellow );
		text4 = text.addTextInstance( "kudos", vec4, 1.0f, 5 );
		
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
