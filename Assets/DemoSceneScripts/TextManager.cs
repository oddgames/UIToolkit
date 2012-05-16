using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TextManager : MonoBehaviour
{
    private UITextInstance text1, text2, text3, text4, text5, text6;
	private UITextInstance textWrap1, textWrap2;
	
	public bool setLowAsciiForcingMode = true;
	public string highAsciiString = char.ConvertFromUtf32( 8220 ) + "Grand Central Station" + char.ConvertFromUtf32( 8221 );
	
	
	void Start()
	{
		// setup our UIText which will parse our .fnt file and allow us to create instances that use it
		var text = new UIText( "prototype", "prototype.png" );

		
		// spawn new text instances showing off the relative positioning features by placing one text instance in each corner
		// Uses default color, scale, alignment, and depth.
		text1 = text.addTextInstance( "hello man.  I have a line\nbreak.", 0, 0 );
        text1.pixelsFromTop( 0, 0 );

		text2 = text.addTextInstance( "testing small bitmap fonts", 0, 0, 0.3f );
        text2.pixelsFromBottomLeft( 2, 2 );
		
		
		// Centering using alignment modes.
		text3 = text.addTextInstance( "with only\n1 draw call\nfor everything", 0, 0, 0.5f, 5, Color.yellow, UITextAlignMode.Right, UITextVerticalAlignMode.Top );
        text3.positionFromTopLeft( 0.5f, 0.2f );

		
		// High Ascii forcing crash demo. To test this, 
		// Disable "Set Low ASCII Forcing Mode" in the TextManager inspector and see the crash.
		// Not as handy if you don't need to paste in large amounts of text from word.
		text.forceLowAscii = setLowAsciiForcingMode;

		text4 = text.addTextInstance( highAsciiString, 0, 0, 0.4f, 1, Color.white, UITextAlignMode.Right, UITextVerticalAlignMode.Bottom );
        text4.positionFromBottomRight( 0f, 0f );
		
		
		// Centering using text size calculation offset and per-char color
		var centeredText = "Be sure to try this with\niPhone and iPad resolutions";
		var colors = new List<Color>();
		for( var i = 0; i < centeredText.Length; i++ )
			colors.Add( Color.Lerp( Color.white, Color.magenta, (float)i / centeredText.Length ) );
		
		text5 = text.addTextInstance( centeredText, 0, 0, 1f, 4, colors.ToArray(), UITextAlignMode.Center, UITextVerticalAlignMode.Middle );
		text5.positionCenter();
		
		
		// Now center on right side.
		text6 = text.addTextInstance( "Vert-Centering on right side", 0, 0, 0.5f, 1, Color.white, UITextAlignMode.Right, UITextVerticalAlignMode.Middle );
        text6.positionFromTopRight( 0.3f, 0f );
		
		
		var wrapText = new UIText( "prototype", "prototype.png" );
		wrapText.wrapMode = UITextLineWrapMode.MinimumLength;
		wrapText.lineWrapWidth = 100.0f;
		textWrap1 = wrapText.addTextInstance( "Testing line wrap width with small words in multiple resolutions.\n\nAnd manual L/B.", 0, 0, 0.3f, 1, Color.white, UITextAlignMode.Left, UITextVerticalAlignMode.Bottom );
        textWrap1.positionFromBottomLeft( 0.1f, 0.1f );
		
		wrapText.lineWrapWidth = 100.0f;
		wrapText.wrapMode = UITextLineWrapMode.AlwaysHyphenate;
		
		textWrap2 = wrapText.addTextInstance( "This should be hyphenated. Check baseline - tytyt", 0, 0, 0.5f, 1, Color.green, UITextAlignMode.Center, UITextVerticalAlignMode.Bottom );
        textWrap2.positionFromBottom( 0f );
		
		StartCoroutine( modifyTextInstances() );
	}
	

	IEnumerator modifyTextInstances()
	{
		yield return new WaitForSeconds( 1.0f );
		text2.text = "gonna change the little text";
		text2.setColorForAllLetters( Color.green );
		
		yield return new WaitForSeconds( 2.0f );
		text3.destroy();
		
		yield return new WaitForSeconds( 1.0f );
		text4.destroy();
		text2.destroy();
		
		yield return new WaitForSeconds( 1.0f );
		textWrap1.destroy();
		textWrap2.destroy();
	}

}
