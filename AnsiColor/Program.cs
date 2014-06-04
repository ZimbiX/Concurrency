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
using System.Collections;
using System.Linq;
using System.Net.Sockets;
using System.Text;

#endregion

namespace AnsiColor
{
    class Program
    {
        static void SendString ( string sendString, NetworkStream networkStream )
        {
            // Grab our bytes to send
            byte [] toSend = System.Text.ASCIIEncoding.ASCII.GetBytes ( sendString );

            // Send our data
            networkStream.Write ( toSend, 0, toSend.Length );
        } // End of SendString

        static void Main ( string [ ] args )
        {
            // Create our tcpListener
            TcpListener tcpListener = new TcpListener ( 5484 );
            // Start listening
            tcpListener.Start ( );
            // Let the user know we are waiting
            Console.WriteLine ( "Waiting for connection. (telnet localhost 5484).\r\n" );
            // Accept a client
            TcpClient client = tcpListener.AcceptTcpClient ( );
            // Get our stream
            NetworkStream networkStream = client.GetStream ( );

            // Send our color demo
            SendString ( AnsiColor.ColorDemo ( ), networkStream );
            // Send our custom demo
            SendString ( AnsiColor.CustomDemo ( ), networkStream );
            // Send our progress bar demo
            SendString ( AnsiColor.ProgressBarDemo ( 10, 10, "{!green}" ), networkStream );
            // Send our progress bar demo
            SendString ( AnsiColor.ProgressBarDemo ( 70, 20, "{!cyan}" ), networkStream );
            // Send our progress bar demo
            SendString ( AnsiColor.ProgressBarDemo ( 100, 50, "{!yellow}" ), networkStream );

            // Wait for input
            Console.WriteLine ( "Sample completed. Hit return to exit." );
            Console.ReadLine ( );
        }
    }
}
