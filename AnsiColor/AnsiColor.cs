/************************************************
 * 
 * Code Written March 2008 by Kyle Hankinson
 * 
 * Any reproduction of this code in any articles or tutorials must give create to the creator.
 * A Full copy of source code can be found online at http://compilr.com/IDE/89-AnsiColors/
 * 
 ***********************************************/

#region Using Directive

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

// Challenges { }'s inside of strings
namespace AnsiColor
{
    public class AnsiColor
    {
        #region Private Varaibles

        // Create our color table
        private static List <ColorData> colorTable = new List <ColorData> ( );

        #endregion

        #region Constructor

        /// <summary>
        /// Our static constructor is used to prefill our color table, so that we do not need
        /// to do so at runtime.
        /// </summary>
        static AnsiColor ( )
        {
            // Our reset values turns everything to the default mode
            colorTable.Add ( new ColorData ( "{reset}",    "\x1B[0m", "Reset" ) );

            // Style Modifiers (on)
            colorTable.Add ( new ColorData ( "{bold}",      "\x1B[1m", "Bold" ) );
            colorTable.Add ( new ColorData ( "{italic}",    "\x1B[3m", "Italic" ) );
            colorTable.Add ( new ColorData ( "{ul}",        "\x1B[4m", "Underline" ) );
            colorTable.Add ( new ColorData ( "{blink}",     "\x1B[5m", "Blink" ) );
            colorTable.Add ( new ColorData ( "{blinkf}",    "\x1B[6m", "Blink Fast" ) );
            colorTable.Add ( new ColorData ( "{inverse}",   "\x1B[7m", "Inverse" ) );
            colorTable.Add ( new ColorData ( "{strike}",    "\x1B[9m", "Strikethrough" ) );

            // Style Modifiers (off)
            colorTable.Add ( new ColorData ( "{!bold}",     "\x1B[22m", "Bold Off" ) );
            colorTable.Add ( new ColorData ( "{!italic}",   "\x1B[23m", "Italic Off" ) );
            colorTable.Add ( new ColorData ( "{!ul}",       "\x1B[24m", "Underline Off" ) );
            colorTable.Add ( new ColorData ( "{!blink}",    "\x1B[25m", "Blink Off" ) );
            colorTable.Add ( new ColorData ( "{!inverse}",  "\x1B[27m", "Inverse Off" ) );
            colorTable.Add ( new ColorData ( "{!strike}",   "\x1B[29m", "Strikethrough Off" ) );

            // Foreground Color
            colorTable.Add ( new ColorData ( "{black}",     "\x1B[30m", "Foreground black" ) );
            colorTable.Add ( new ColorData ( "{red}",       "\x1B[31m", "Foreground red" ) );
            colorTable.Add ( new ColorData ( "{green}",     "\x1B[32m", "Foreground green" ) );
            colorTable.Add ( new ColorData ( "{yellow}",    "\x1B[33m", "Foreground yellow" ) );
            colorTable.Add ( new ColorData ( "{blue}",      "\x1B[34m", "Foreground blue" ) );
            colorTable.Add ( new ColorData ( "{magenta}",   "\x1B[35m", "Foreground magenta" ) );
            colorTable.Add ( new ColorData ( "{cyan}",      "\x1B[36m", "Foreground cyan" ) );
            colorTable.Add ( new ColorData ( "{white}",     "\x1B[37m", "Foreground white" ) );

            // Background Color
            colorTable.Add ( new ColorData ( "{!black}",    "\x1B[40m", "Background black" ) );
            colorTable.Add ( new ColorData ( "{!red}",      "\x1B[41m", "Background red" ) );
            colorTable.Add ( new ColorData ( "{!green}",    "\x1B[42m", "Background green" ) );
            colorTable.Add ( new ColorData ( "{!yellow}",   "\x1B[43m", "Background yellow" ) );
            colorTable.Add ( new ColorData ( "{!blue}",     "\x1B[44m", "Background blue" ) );
            colorTable.Add ( new ColorData ( "{!magenta}",  "\x1B[45m", "Background magenta" ) );
            colorTable.Add ( new ColorData ( "{!cyan}",     "\x1B[46m", "Background cyan" ) );
            colorTable.Add ( new ColorData ( "{!white}",    "\x1B[47m", "Background white" ) );
        } // End of AnsiColor

        #endregion

        #region Helper Methods

        /// <summary>
        /// The colorize string will take in a string that contains metadata for ansi colors. It will
        /// then replace all of the metadata with the actual color identifiers.
        /// </summary>
        /// <param name="stringToColor"></param>
        /// <returns></returns>
        public static string Colorize ( string stringToColor )
        {
            // Loop through our table
            foreach ( ColorData colorData in colorTable )
                // Replace our identifier with our code
                stringToColor = stringToColor.Replace ( colorData.Identifier, colorData.Code );

            // Return our colored string
            return ( stringToColor );
        } // End of Colorize Function

        #endregion

        #region Demo Methods

        public static string ColorDemo ( )
        {
            StringBuilder output = new StringBuilder ( );

            // Loop through all of our colors
            foreach ( ColorData colorData in colorTable )
                // Append our test string
                output.AppendFormat ( "Displaying {0,-20} {1}TEST{{reset}}\r\n", colorData.Definition, colorData.Identifier );

            // Return our output
            return ( Colorize ( output.ToString ( ) + "\r\n\r\n" ) );
        } // End of ColorDemo

        public static string CustomDemo ( )
        {
            return ( Colorize (
                "{bold}{red}Bold Red\r\n{blue}Bold Blue\r\n{!red}With red background{reset}\r\n" ) );
        } // End of CustomDemo

        public static string ProgressBarDemo ( int percent, int width, string colorCode )
        {
            // Our max size
            int MaxSize = 10;

            // Make sure our percent is valid. If it is not force it to zero
            if ( percent < 0 || percent > 100 ) percent = 0;

            // Calculate how many colored blocks we should have
            int blocks = ( int ) ( ( percent / 100f ) * width );

            // Default our progress bar to start with a red background
            string progressBar = "{" + colorCode;
            // Add our blocks
            for ( int i = 0; i < blocks; i++ )
                // Append our space
                progressBar += ' ';

            // Back to normal
            progressBar += "{reset}";

            // Add our blocks
            for ( int i = 0; i < width - blocks; i++ )
                // Append our space
                progressBar += ' ';
            // Append our final char
            progressBar += '}';

            // Return our progress bar
            return ( "Progress Bar: " + Colorize ( progressBar ) + "\r\n" );
        } // End of ProgressBarDemo

        #endregion
    }

    struct ColorData
    {
        #region Private Variables

        string identifier;
        string code;
        string definition;

        #endregion

        #region Public Properties

        public string Code
        {
            get { return code; }
        } // End of ReadOnly Code

        public string Identifier
        {
            get { return identifier; }
        } // End of ReadOnly Identifier

        public string Definition
        {
            get { return definition; }
        } // End of ReadOnly Definition

        #endregion

        public ColorData ( string identifier, string code, string definition )
        {
            // Set our values
            this.identifier = identifier;
            this.code = code;
            this.definition = definition;
        } // End of ColorData Constructor

    } // End of ColorData structure
}
