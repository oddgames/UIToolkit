using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TextManager : MonoBehaviour
{
	private UITextInstance text2;
	private UITextInstance text3;
	private UITextInstance text4;
	
	private UITextInstance textWrap1;
	private UITextInstance textWrap2;
	
	public bool setLowAsciiForcingMode = true;
	public string highAsciiString = char.ConvertFromUtf32( 8220 ) + "Grand Central Station" + char.ConvertFromUtf32( 8221 );
	
	void Start()
	{
		// setup our text instance which will parse our .fnt file and allow us to
		var text = new UIText( "prototype", "prototype.png" );
		
		
		// spawn new text instances showing off the relative positioning features by placing one text instance in each corner
		var x = UIRelative.xPercentFrom( UIxAnchor.Left, 0f );
		var y = UIRelative.yPercentFrom( UIyAnchor.Top, 0f );
		// Uses default color, scale, alignment, and depth.
		text.addTextInstance( "hello man.  I have a line\nbreak.", x, y );
		
		var textSize = text.sizeForText( "testing small bitmap fonts", 0.3f );
		x = UIRelative.xPercentFrom( UIxAnchor.Left, 0f );
		y = UIRelative.yPercentFrom( UIyAnchor.Bottom, 0f, textSize.y );
		text2 = text.addTextInstance( "testing small bitmap fonts", x, y, 0.3f );
		
		
		// Centering using alignment modes.
		x = UIRelative.xPercentFrom( UIxAnchor.Right, 0f );
		y = UIRelative.yPercentFrom( UIyAnchor.Top, 0f );
		text3 = text.addTextInstance( "with only\n1 draw call\nfor everything", x, y, 0.5f, 5, Color.yellow, UITextAlignMode.Right, UITextVerticalAlignMode.Top );
		
		
		// High Ascii forcing crash demo. To test this, 
		// Disable "Set Low ASCII Forcing Mode" in the TextManager inspector and see the crash.
		// Not as handy if you don't need to paste in large amounts of text from word.
		text.forceLowAscii = setLowAsciiForcingMode;
		
		x = UIRelative.xPercentFrom( UIxAnchor.Right, 0, 0 );
		y = UIRelative.yPercentFrom( UIyAnchor.Bottom, 0, 0 );
		text4 = text.addTextInstance( highAsciiString, x, y, 0.4f, 1, Color.white, UITextAlignMode.Right, UITextVerticalAlignMode.Bottom );
		
		
		// Centering using text size calculation offset and per-char color
		var centeredText = "Be sure to try this with\niPhone and iPad resolutions";
		var colors = new List<Color>();
		for( var i = 0; i < centeredText.Length; i++ )
			colors.Add( Color.Lerp( Color.white, Color.magenta, (float)i / centeredText.Length ) );
		
		textSize = text.sizeForText( centeredText );
		var center = UIRelative.center( textSize.x, textSize.y );
		text.addTextInstance( centeredText, center.x, center.y, 1f, 4, colors.ToArray(), UITextAlignMode.Left, UITextVerticalAlignMode.Top );
		
		
		// Now center on right side.
		x = UIRelative.xPercentFrom( UIxAnchor.Right, 0 );
		y = UIRelative.yPercentFrom( UIyAnchor.Top, 0.5f );
		text.addTextInstance( "Vert-Centering on right side", x, y, 0.5f, 1, Color.white, UITextAlignMode.Right, UITextVerticalAlignMode.Middle );
		
		
		x = UIRelative.xPercentFrom( UIxAnchor.Left, 0f );
		y = UIRelative.yPercentFrom( UIyAnchor.Bottom, 0f, textSize.y * 3 );

		var wrapText = new UIText( "prototype", "prototype.png" );
		wrapText.wrapMode = UITextLineWrapMode.MinimumLength;
		wrapText.lineWrapWidth = 100.0f;
		
		textWrap1 = wrapText.addTextInstance( "Testing line wrap width with small words in multiple resolutions.\n\nAnd manual L/B.", x, y, 0.3f, 1, Color.white, UITextAlignMode.Left, UITextVerticalAlignMode.Bottom);
		
		wrapText.lineWrapWidth = 100.0f;
		wrapText.wrapMode = UITextLineWrapMode.AlwaysHyphenate;
		
		x = UIRelative.xPercentFrom( UIxAnchor.Left, 0.5f );
		y = UIRelative.yPercentFrom( UIyAnchor.Bottom, 0f );
		textWrap2 = wrapText.addTextInstance( "This should be hyphenated. Check baseline - tytyt", x, y, 0.5f, 1, Color.green, UITextAlignMode.Center, UITextVerticalAlignMode.Bottom );
		
		StartCoroutine( modifyTextInstances() );
	}
	

	IEnumerator modifyTextInstances()
	{
		yield return new WaitForSeconds( 1.0f );
		text2.text = "gonna change the little text";
		text2.setColorForAllLetters( Color.green );
		
		yield return new WaitForSeconds( 2.0f );
		text3.clear();
		
		yield return new WaitForSeconds( 1.0f );
		text4.clear();
		text2.clear();
		
		yield return new WaitForSeconds( 1.0f );
		textWrap1.clear();
		textWrap2.clear();
	}

}
