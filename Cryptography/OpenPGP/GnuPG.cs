/*
 *  Authors:  Benton Stark
 * 
 *  Copyright (c) 2007-2012 Starksoft, LLC (http://www.starksoft.com) 
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 */


using System;
using System.Text; 
using System.Diagnostics; 
using System.IO; 
using System.Threading;
using System.Globalization;
using Microsoft.Win32;
using System.Collections;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;

namespace Starksoft.Cryptography.OpenPGP
{

    /// <summary>
    /// GnuPG output itemType.
    /// </summary>
    public enum OutputTypes
    {
        /// <summary>
        /// Ascii armor output.
        /// </summary>
        AsciiArmor,
        /// <summary>
        /// Binary output.
        /// </summary>
        Binary
    };   
        
    /// <summary>
    /// Interface class for GnuPG.
    /// </summary>
    /// <remarks>
    /// <para>
    /// GNU Privacy Guard from the GNU Project (also called GnuPG or GPG for short) is a highly regarded and supported opensource project that provides a complete and free implementation of the OpenPGP standard as defined by RFC2440. 
    /// GnuPG allows you to encrypt and sign your data and communication, manage your public and privde OpenPGP keys as well 
    /// as access modules for all kind of public key directories. GPG.EXE and GPG2.EXE, is a command line tool that is installed with GnuPG and contains features for easy integration with other applications. 
    /// </para>
    /// <para>
    /// The Starksoft OpenPGP Component for .NET provides classes that interface with the GPG.EXE command line tool.  The Starksoft OpenPGP libraries allows any .NET application to use GPG.EXE to encrypt or decypt data using
    /// .NET IO Streams.  No temporary files are required and everything is handled through streams.  Any .NET Stream object can be used as long as the source stream can be read and the 
    /// destination stream can be written to.  But, in order for the Starksoft OpenPGP Component for .NET to work you must first install the lastest version of GnuPG which includes GPG.EXE.  
    /// You can obtain the latest version at http://www.gnupg.org/.  See the GPG.EXE or GPG2.EXE tool documentation for information
    /// on how to add keys to the GPG key ring and creating your public and private keys.
    /// </para>
    /// <para>
    /// If you are new to GnuPG please install the application and then read how to generate new key pair or importing existing OpenPGP keys. 
    /// You can read more about key generation and importing at http://www.gnupg.org/gph/en/manual.html#AEN26
    /// </para>
    /// <para>
    /// Encrypt File Example:
    /// <code>
    /// // create a new GnuPG object
    /// GnuPG gpg = new GnuPG();
    /// // specify a recipient that is already on the key-ring 
    /// gpg.Recipient = "myfriend@domain.com";
    /// // create an IO.Stream object to the source of the data and open it
    /// FileStream sourceFile = new FileStream(@"c:\temp\source.txt", FileMode.Open);
    /// // create an IO.Stream object to a where I want the encrypt data to go
    /// FileStream outputFile = new FileStream(@"c:\temp\output.txt", FileMode.Create);
    /// // encrypt the data using IO Streams - any type of input and output IO Stream can be used
    /// // as long as the source (input) stream can be read and the destination (output) stream 
    /// // can be written to
    /// gpg.Encrypt(sourceFile, outputFile);
    /// // close the files
    /// sourceFile.Close();
    /// outputFile.Close();
    /// </code>
    /// </para>
    /// <para>
    /// Decrypt File Example:
    /// <code>
    /// // create a new GnuPG object
    /// GnuPG gpg = new GnuPG();
    /// // create an IO.Stream object to the encrypted source of the data and open it 
    /// FileStream encryptedFile = new FileStream(@"c:\temp\output.txt", FileMode.Open);
    /// // create an IO.Stream object to a where you want the decrypted data to go
    /// FileStream unencryptedFile = new FileStream(@"c:\temp\unencrypted.txt", FileMode.Create);
    /// // specify our credentials (key id and secret passphrase)
    /// NetworkCredential credential = new NetworkCredential("B20A4563", "secret passphrase");
    /// gpg.Credential = credential;            
    /// // decrypt the data using IO Streams - any type of input and output IO Stream can be used
    /// // as long as the source (input) stream can be read and the destination (output) stream 
    /// // can be written to
    /// gpg.Decrypt(encryptedFile, unencryptedFile);
    /// // close the files
    /// encryptedFile.Close();
    /// unencryptedFile.Close();
    /// </code>
    /// </para>
    /// </remarks>
	public class GnuPG : IDisposable 
	{
        private NetworkCredential _credential;
        private string _recipient;
		private string _homePath;
        private string _binaryPath;
        private OutputTypes _outputType;
		private int _timeout = 10000; // 10 seconds
		private Process _proc;

        private Stream _outputStream;
        private Stream _errorStream;

        private const string GPG_EXECUTABLE_V1 = "gpg.exe";
        private const string GPG_EXECUTABLE_V2 = "gpg2.exe";
        private const string GPG_REGISTRY_KEY_UNINSTALL_V1 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\GnuPG";
        private const string GPG_REGISTRY_KEY_UNINSTALL_V2 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\GPG4Win";
        private const string GPG_REGISTRY_VALUE_INSTALL_LOCATION = "InstallLocation";
        private const string GPG_REGISTRY_VALUE_DISPLAYVERSION = "DisplayVersion";
        private const string GPG_COMMON_INSTALLATION_PATH = @"C:\Program Files\GNU\GnuPG";

        /// <summary>
        /// GnuPG actions.
        /// </summary>
		private enum ActionTypes
		{ 
            /// <summary>
            /// Encrypt data.
            /// </summary>
			Encrypt, 
            /// <summary>
            /// Decrypt data.
            /// </summary>
			Decrypt,
            /// <summary>
            /// Sign data.
            /// </summary>
            Sign,
            /// <summary>
            /// Verify signed data.
            /// </summary>
            Verify,
            /// <summary>
            /// Import a key
            /// </summary>
            Import
		};

        /// <summary>
        /// GnuPG interface class default constructor.
        /// </summary>
        /// <remarks>
        /// The GPG executable location is obtained by information in the windows registry.  Home path is set to the same as the
        /// GPG executable path.  Output itemType defaults to Ascii Armour.
        /// </remarks>
     	public GnuPG()
		{
            SetDefaults();
        }

        /// <summary>
        /// GnuPG interface class constuctor.
        /// </summary>
        /// <remarks>Output itemType defaults to Ascii Armour.</remarks>
        /// <param name="homePath">The home directory where files to encrypt and decrypt are located.</param>
        /// <param name="binaryPath">The GnuPG executable binary directory.</param>
		public GnuPG(string homePath, string binaryPath)
		{
            _homePath = homePath;
            _binaryPath = binaryPath;
            SetDefaults();
		}

        /// <summary>
        /// GnuPG interface class constuctor.
        /// </summary>
        /// <param name="homePath">The home directory where files to encrypt and decrypt are located.</param>
        /// <remarks>
        /// The GPG executable location is obtained by information in the windows registry.  Output itemType defaults to Ascii Armour.
        /// </remarks>
        public GnuPG(string homePath)
        {
            _homePath = homePath;
            SetDefaults();
        }

        /// <summary>
        /// Get or set the timeout value for the GnuPG operations in milliseconds. 
        /// </summary>
        /// <remarks>
        /// The default timeout is 10000 milliseconds (10 seconds).
        /// </remarks>
		public int Timeout
		{
			get{ return(_timeout);	}
			set{ _timeout = value;	}
		}

        /// <summary>
        /// Recipient name of the encrypted data.
        /// </summary>
        public string Recipient
        {
            get { return _recipient; }
            set { _recipient = value; }
        }

        /// <summary>
        /// Secret credential (key id and passphrase)
        /// </summary>
        public NetworkCredential Credential
        {
            get { return _credential; }            
            set 
            { 
                if (value != null)
                {
                    if (GetFingerprint(value.UserName) == string.Empty)
                    {
                        throw new ArgumentException(string.Format("Cannot set credential to {0} - no such identity", value.UserName));
                    }
                }
                _credential = value; 
            }
        }

        /// <summary>
        /// The itemType of output that GPG should generate.
        /// </summary>
        public OutputTypes OutputType
        {
            get { return _outputType; }
            set { _outputType = value; }
        }

        /// <summary>
        /// Path to your home directory.
        /// </summary>
        public string HomePath
        {
            get { return _homePath; }
            set { _homePath = value; }
        }

        /// <summary>
        /// Path to the location of the GPG.EXE binary executable.
        /// </summary>
        public string BinaryPath
        {
            get { return _binaryPath; }
            set { _binaryPath = value; }
        }

        private void VerifyCredentialIsSet()
        {
            if (_credential == null || _credential.SecurePassword.Length == 0)
                throw new GnuPGException("Credential must be set");
        }

        /// <summary>
        /// Encrypt OpenPGP data using IO Streams.
        /// </summary>
        /// <param name="inputStream">Input stream data containing the data to encrypt.</param>
        /// <param name="outputStream">Output stream which will contain encrypted data.</param>
        /// <param name="metadataStream">Output stream which will contain GPG metadata about the operation.</param>
        /// <remarks>
        /// You must add the recipient's public key to your GnuPG key ring before calling this method.  Please see the GnuPG documentation for more information.
        /// </remarks>
        public void Encrypt(Stream inputStream, Stream outputStream, Stream metadataStream)
        {
            if (inputStream == null)
                throw new ArgumentNullException("Argument inputStream can not be null.");

            if (outputStream == null)
                throw new ArgumentNullException("Argument outputStream can not be null.");
            
            if (!inputStream.CanRead)
                throw new ArgumentException("Argument inputStream must be readable.");

            if (!outputStream.CanWrite)
                throw new ArgumentException("Argument outputStream must be writable.");

            VerifyCredentialIsSet();

            StringBuilder options = GetCmdLineSwitches(ActionTypes.Encrypt, true);
            ExecuteGPG(options, inputStream, outputStream, metadataStream);
        }

        /// <summary>
        /// Decrypt OpenPGP data using IO Streams.
        /// </summary>
        /// <param name="inputStream">Input stream containing encrypted data.</param>
        /// <param name="outputStream">Output stream which will contain decrypted data.</param>
        /// <param name="metadataStream">Output stream which will contain GPG metadata about the operation.</param> 
        public void Decrypt(Stream inputStream, Stream outputStream, Stream metadataStream)
        {
            if (inputStream == null)
                throw new ArgumentNullException("Argument inputStream can not be null.");

            if (outputStream == null)
                throw new ArgumentNullException("Argument outputStream can not be null.");

            if (!inputStream.CanRead)
                throw new ArgumentException("Argument inputStream must be readable.");

            if (!outputStream.CanWrite)
                throw new ArgumentException("Argument outputStream must be writable.");

            VerifyCredentialIsSet();

            StringBuilder options = GetCmdLineSwitches(ActionTypes.Decrypt, true);
            ExecuteGPG(options, inputStream, outputStream, metadataStream);
        }

        /// <summary>
        /// Sign input stream data with default user key.
        /// </summary>
        /// <param name="inputStream">Input stream containing data to sign.</param>
        /// <param name="outputStream">Output stream containing signed data.</param>
        /// <param name="outputStream">Output stream which will contain GPG metadata about the operation.</param>        
        public void Sign(Stream inputStream, Stream outputStream, Stream metadataStream)
        {
            if (inputStream == null)
                throw new ArgumentNullException("Argument inputStream can not be null.");

            if (outputStream == null)
                throw new ArgumentNullException("Argument outputStream can not be null.");

            if (!inputStream.CanRead)
                throw new ArgumentException("Argument inputStream must be readable.");

            if (!outputStream.CanWrite)
                throw new ArgumentException("Argument outputStream must be writable.");

            VerifyCredentialIsSet();

            StringBuilder options = GetCmdLineSwitches(ActionTypes.Sign, true);
            ExecuteGPG(options, inputStream, outputStream, metadataStream);
        }

        /// <summary>
        /// Verify signed input stream data with default user key.
        /// </summary>
        /// <param name="inputStream">Input stream containing signed data to verify.</param>
        public void Verify(Stream inputStream)
        {
            if (inputStream == null)
                throw new ArgumentNullException("Argument inputStream can not be null.");

            if (!inputStream.CanRead)
                throw new ArgumentException("Argument inputStream must be readable.");

            StringBuilder options = GetCmdLineSwitches(ActionTypes.Verify, false);
            ExecuteGPG(options, inputStream, new MemoryStream(), new MemoryStream());
        }

        /// <summary>
        /// Import key from the input stream
        /// </summary>
        /// <param name="inputStream">Input stream containing key data</param>
        /// <returns>Key ID</returns>
        public string Import(Stream inputStream)
        {
            if (inputStream == null)
                throw new ArgumentNullException("Argument inputStream can not be null.");

            if (!inputStream.CanRead)
                throw new ArgumentException("Argument inputStream must be readable.");

            MemoryStream outputStream = new MemoryStream();
            MemoryStream metadataStream = new MemoryStream();

            StringBuilder options = GetCmdLineSwitches(ActionTypes.Import, false);
            ExecuteGPG(options, inputStream, outputStream, metadataStream, false);
            StreamReader reader = new StreamReader(metadataStream);
            reader.BaseStream.Position = 0;
            string output = reader.ReadToEnd();
            Match match = Regex.Match(output, @"key ([A-Z0-9]+):", RegexOptions.Multiline);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                throw new GnuPGException("Error importing key");
            }
        }

        /// <summary>
        /// Retrieves a collection of secret keys from the GnuPG application.
        /// </summary>
        /// <returns>Collection of GnuPGKey objects.</returns>
        public GnuPGKeyCollection GetSecretKeys()
        {
            return new GnuPGKeyCollection(GetCommand("--list-secret-keys"));
        }

        /// <summary>
        /// Delete the specified key from the keyring
        /// </summary>
        /// <param name="keyID">Key ID to delete</param>
        public void DeleteKey(string keyID)
        {
            if (keyID == null || keyID == string.Empty)
                throw new ArgumentException("Argument keyID can not be null or empty");

            StreamReader sr = GetCommand(string.Format("--batch --yes --delete-key {0}", keyID));
            sr.ReadToEnd();
        }

        /// <summary>
        /// Export a key (as text) and return it
        /// </summary>
        /// <param name="keyID">Key id from a GnuPGKey</param>
        /// <returns>String representation of the key</returns>
        public string GetActualKey(string keyID)
        {
            if (keyID == null || keyID == string.Empty)
                throw new ArgumentException("Argument keyID can not be null or empty");

            StreamReader sr = GetCommand(string.Format("--export -a {0}", keyID));
            return sr.ReadToEnd();
        }

        public void SignKey(string keyID)
        {
            if (keyID == null || keyID == string.Empty)
                throw new ArgumentException("Argument keyID can not be null or empty");

            VerifyCredentialIsSet();

            StreamReader sr = GetCommand(string.Format("--yes --sign-key {0}", keyID), true);
            sr.ReadToEnd();
        }

        /// <summary>
        /// Get the fingerprint of the key (must be imported first)
        /// </summary>
        /// <param name="keyID">Key id from a GnuPGKey</param>
        /// <returns>String representation of the fingerprint, including spaces (ex. "9DC5 4DB6 ...")</returns>
        public string GetFingerprint(string keyID)
        {
            if (keyID == null || keyID == string.Empty)
                throw new ArgumentException("Argument keyID can not be null or empty");

            StreamReader sr = GetCommand(string.Format("--fingerprint {0}", keyID));
            string output = sr.ReadToEnd();
            Match match = Regex.Match(output, @"Key fingerprint = ([\w\d ]+)", RegexOptions.Multiline);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Retrieves a collection of all keys from the GnuPG application.
        /// </summary>
        /// <returns>Collection of GnuPGKey objects.</returns>
        public GnuPGKeyCollection GetKeys()
        {
            return new GnuPGKeyCollection(GetCommand("--list-keys"));
        }

        private StreamReader GetCommand(string command, bool needsPassword = false, Stream inputStream = null)
        {
            StringBuilder options = SetStandardOptions(needsPassword);
            options.Append(command);

            // get the full path to either GPG.EXE or GPG2.EXE
            string gpgPath = GetGnuPGPath();
                        
            //  create a process info object with command line options
            ProcessStartInfo procInfo = new ProcessStartInfo(gpgPath, options.ToString());

            //  init the procInfo object
            procInfo.CreateNoWindow = true;
            procInfo.UseShellExecute = false;
            procInfo.RedirectStandardInput = true;
            procInfo.RedirectStandardOutput = true;
            procInfo.RedirectStandardError = true;

            MemoryStream outputStream = new MemoryStream();

            try
            {
                //  start the gpg process and get back a process start info object
                _proc = Process.Start(procInfo);
                
                if (needsPassword)
                {
                    //  push passphrase onto stdin with a CRLF                    
                    _proc.StandardInput.WriteLine(_credential.Password);
                    _proc.StandardInput.Flush();
                }

                if (inputStream != null)
                {
                    //  copy the input stream to the process standard input object
                    CopyStream(inputStream, _proc.StandardInput.BaseStream);
                    _proc.StandardInput.Flush();
                }

                // close the process standard input object
                _proc.StandardInput.Close();

                //  wait for the process to return with an exit code (with a timeout variable)
                if (!_proc.WaitForExit(Timeout))
                {
                    throw new GnuPGException("A time out event occurred while executing the GPG program.");
                }

                //  if the process exit code is not 0 then read the error text from the gpg.exe process and throw an exception
                if (_proc.ExitCode != 0)
                    throw new GnuPGException(_proc.StandardError.ReadToEnd());

                CopyStream(_proc.StandardOutput.BaseStream, outputStream);
            }
            catch (GnuPGException)
            {
                throw;
            }
            catch (Exception exp)
            {
                throw new GnuPGException(String.Format("An error occurred while trying to execute command {0}.", command, exp));
            }
            finally
            {
                Dispose(true);
            }

            StreamReader reader = new StreamReader(outputStream);
            reader.BaseStream.Position = 0;
            return reader;
        }

        private StringBuilder SetStandardOptions(bool needsPassword)
        {
            StringBuilder options = new StringBuilder();

            // set a home directory if the user specifies one
            if (_homePath != null && _homePath.Length != 0)
                options.Append(String.Format(CultureInfo.InvariantCulture, "--homedir \"{0}\" ", _homePath));

            if (needsPassword)
            {
                //  read the passphrase from the standard input
                options.Append("--passphrase-fd 0 ");
            }

            // turn off verbose statements
            options.Append("--no-verbose --batch ");

            // always use the trusted model so we don't get an interactive session with gpg.exe
            options.Append("--trust-model always ");

            return options;
        }

        private StringBuilder GetCmdLineSwitches(ActionTypes action, bool requiresPassword = true)
        {
            StringBuilder options = SetStandardOptions(requiresPassword);

            //  handle the action
            switch (action)
            {
                case ActionTypes.Encrypt:
                    if (_recipient == null && action == ActionTypes.Encrypt)
                        throw new GnuPGException("A Recipient is required before encrypting data.  Please specify a valid recipient using the Recipient property on the GnuPG object.");

                    //  check to see if the user wants ascii armor output or binary output (binary is the default mode for gpg)
                    if (_outputType == OutputTypes.AsciiArmor)
                        options.Append("--armor ");
                    options.Append(String.Format(CultureInfo.InvariantCulture, "--recipient \"{0}\" --encrypt", _recipient));
                    break;
                case ActionTypes.Decrypt:
                    options.Append("--decrypt ");
                    break;
                case ActionTypes.Sign:
                    options.Append("--sign ");
                    break;
                case ActionTypes.Verify:
                    options.Append("--verify ");
                    break;
                case ActionTypes.Import:
                    options.Append("--import ");
                    break;
            }

            return options;
        }

		private void ExecuteGPG(StringBuilder options, Stream inputStream, Stream outputStream, Stream metadataStream, bool needsPassword = true)
		{
            string gpgErrorText = string.Empty;

            string gpgPath = GetGnuPGPath();

            //  create a process info object with command line options
			ProcessStartInfo procInfo = new ProcessStartInfo(gpgPath, options.ToString());
			
            //  init the procInfo object
			procInfo.CreateNoWindow = true; 
			procInfo.UseShellExecute = false;  
			procInfo.RedirectStandardInput = true;
			procInfo.RedirectStandardOutput = true;
			procInfo.RedirectStandardError = true;

            try
            {
                //  start the gpg process and get back a process start info object
                _proc = Process.Start(procInfo);

                if (needsPassword)
                {
                    //  push passphrase onto stdin with a CRLF
					_proc.StandardInput.WriteLine(GetPassword(_credential));
                    _proc.StandardInput.Flush();
                }
                
                _outputStream = outputStream;
                _errorStream = new MemoryStream();

                // set up threads to run the output stream and error stream asynchronously
                ThreadStart outputEntry = new ThreadStart(AsyncOutputReader);
                Thread outputThread = new Thread(outputEntry);
                outputThread.Name = "GnuPG Output Thread";
                outputThread.Start();
                ThreadStart errorEntry = new ThreadStart(AsyncErrorReader);
                Thread errorThread = new Thread(errorEntry);
                errorThread.Name = "GnuPG Error Thread";
                errorThread.Start();

                //  copy the input stream to the process standard input object
                CopyStream(inputStream, _proc.StandardInput.BaseStream);                                
                _proc.StandardInput.Flush();
               
                // close the process standard input object
                _proc.StandardInput.Close();

                //  wait for the process to return with an exit code (with a timeout variable)
                if (!_proc.WaitForExit(_timeout))
                {
                    throw new GnuPGException("A time out event occurred while executing the GPG program.");
                }

                if (!outputThread.Join(_timeout / 2))
                    outputThread.Abort();

                if (!errorThread.Join(_timeout / 2))
                    errorThread.Abort();

                _errorStream.Position = 0;
                StreamReader rerror = new StreamReader(_errorStream);
                string errorOutput = rerror.ReadToEnd();
                
                if (_proc.ExitCode == 0)
                {
                    // If the process succeeded, treat error output as metadata
                    if (metadataStream != null)
                    {
                        StreamWriter writer = new StreamWriter(metadataStream);
                        writer.Write(errorOutput);
                        writer.Flush();
                    }
                }
                else
                {
                    gpgErrorText = errorOutput; // Actual processerror
                }
            }
            catch (Exception exp)
            {
                throw new GnuPGException(String.Format(CultureInfo.InvariantCulture, "An error occurred while trying to execute GnuPG command {0}", options.ToString(), exp));
            }
            finally
            {
                Dispose();
            }

            // throw an exception with the error information from the gpg.exe process
            if (gpgErrorText.IndexOf("bad passphrase") != -1)
                throw new GnuPGBadPassphraseException(gpgErrorText);

            if (gpgErrorText.Length > 0)
                throw new GnuPGException(gpgErrorText);
        }

		private string GetPassword(NetworkCredential credential)
		{
			if (!string.IsNullOrEmpty(credential.Password))
				return credential.Password;
			else
			{
				// Decrypt the password and return it as a managed string. This is a security problem, but
				// eventually we need the password and need to read it somewhere when passing out to GPG
				var bstrPtr = Marshal.SecureStringToBSTR(credential.SecurePassword);
				var insecurePassword = Marshal.PtrToStringBSTR(bstrPtr);	
				Marshal.ZeroFreeBSTR(bstrPtr);
				return insecurePassword;
			}
		}

        private string GetGnuPGPath()
        {
            // if a full path is provided then use that
            if (!String.IsNullOrEmpty(_binaryPath))
            {
                if (!File.Exists(_binaryPath))
                    throw new GnuPGException(String.Format("binary path to GnuPG executable invalid or file permissions do not allow access: {0}", _binaryPath));
                return _binaryPath;
            }


            // try to find the Windows registry key that contains GPG.EXE (version 1)
            string pathv1 = "";
            RegistryKey hKeyLM_1 = Registry.LocalMachine;
            try
            {
                hKeyLM_1 = hKeyLM_1.OpenSubKey(GPG_REGISTRY_KEY_UNINSTALL_V1);
                pathv1 = (string)hKeyLM_1.GetValue(GPG_REGISTRY_VALUE_INSTALL_LOCATION);
                Path.Combine(pathv1, GPG_EXECUTABLE_V1);
            }
            finally
            {
                if (hKeyLM_1 != null)
                    hKeyLM_1.Close();
            }

            // try to find the Windows registry key that contains GPG2.EXE (version 2)
            string pathv2 = "";
            RegistryKey hKeyLM_2 = Registry.LocalMachine;
            try
            {
                hKeyLM_2 = hKeyLM_2.OpenSubKey(GPG_REGISTRY_KEY_UNINSTALL_V2);
                pathv2 = (string)hKeyLM_2.GetValue(GPG_REGISTRY_VALUE_INSTALL_LOCATION);
                Path.Combine(pathv2, GPG_EXECUTABLE_V2);
            }
            finally
            {
                if (hKeyLM_2 != null)
                    hKeyLM_2.Close();
            }
            
            // try to figure out which path is good
            if (File.Exists(pathv2))
                return pathv2;
            else if (File.Exists(pathv1))
                return pathv1;
            else if (File.Exists(GPG_COMMON_INSTALLATION_PATH))
                return GPG_COMMON_INSTALLATION_PATH;
            else
                throw new GnuPGException("cannot find a valid GPG.EXE or GPG2.EXE file path - set the property 'BinaryPath' to specify a hard path to the executable or verify file permissions are correct.");
        }

        private void CopyStream(Stream input, Stream output)
        {
            if (_asyncWorker != null && _asyncWorker.CancellationPending)
                return;                 

            const int BUFFER_SIZE = 4096;
            byte[] bytes = new byte[BUFFER_SIZE];
            int i;
            while ((i = input.Read(bytes, 0, bytes.Length)) != 0)
            {
                if (_asyncWorker != null && _asyncWorker.CancellationPending)
                    break;                 
                output.Write(bytes, 0, i);
            }
        }

        private void SetDefaults()
        {
            _outputType = OutputTypes.AsciiArmor;
        }

        
        private void AsyncOutputReader()
        {
            Stream input = _proc.StandardOutput.BaseStream;
            Stream output = _outputStream;

            const int BUFFER_SIZE = 4096;
            byte[] bytes = new byte[BUFFER_SIZE];
            int i;
            while ((i = input.Read(bytes, 0, bytes.Length)) != 0)
            {
                output.Write(bytes, 0, i);
            }
        }

        private void AsyncErrorReader()
        {
            if (_proc == null)
                return;
            
            Stream input = _proc.StandardError.BaseStream;
            Stream output = _errorStream;

            const int BUFFER_SIZE = 4096;
            byte[] bytes = new byte[BUFFER_SIZE];
            int i;
            while ((i = input.Read(bytes, 0, bytes.Length)) != 0)
            {
                output.Write(bytes, 0, i);
            }

        }

        /// <summary>
        /// Dispose method for the GnuPG inteface class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose method for the GnuPG interface class.
        /// </summary>       
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_proc != null)
                {
                    //  close all the streams except for our the output stream
                    _proc.StandardInput.Close();
                    _proc.StandardOutput.Close();
                    _proc.StandardError.Close(); 
                    _proc.Close();
                }
            }

            if (_proc != null)
            {
                _proc.Dispose();
                _proc = null;
            }
        }

        /// <summary>
        /// Destructor method for the GnuPG interface class.
        /// </summary>
        ~GnuPG()
        {
          Dispose (false);
        }

#region Asynchronous Methods

        private BackgroundWorker _asyncWorker;
        private Exception _asyncException;
        bool _asyncCancelled;

        /// <summary>
        /// Gets a value indicating whether an asynchronous operation is running.
        /// </summary>
        /// <remarks>Returns true if an asynchronous operation is running; otherwise, false.
        /// </remarks>
        public bool IsBusy
        {
            get { return _asyncWorker == null ? false : _asyncWorker.IsBusy; }
        }

        /// <summary>
        /// Gets a value indicating whether an asynchronous operation is cancelled.
        /// </summary>
        /// <remarks>Returns true if an asynchronous operation is cancelled; otherwise, false.
        /// </remarks>
        public bool IsAsyncCancelled
        {
            get { return _asyncCancelled; }
        }

        /// <summary>
        /// Cancels any asychronous operation that is currently active.
        /// </summary>
        public void CancelAsync()
        {
            if (_asyncWorker != null && !_asyncWorker.CancellationPending && _asyncWorker.IsBusy)
            {
                _asyncCancelled = true;
                _asyncWorker.CancelAsync();
            }
        }

        private void CreateAsyncWorker()
        {
            if (_asyncWorker != null)
                _asyncWorker.Dispose();
            _asyncException = null;
            _asyncWorker = null;
            _asyncCancelled = false;
            _asyncWorker = new BackgroundWorker();
        }

        /// <summary>
        /// Event handler for EncryptAsync method completed.
        /// </summary>
        public event EventHandler<EncryptAsyncCompletedEventArgs> EncryptAsyncCompleted;

        /// <summary>
        /// Starts asynchronous execution to encrypt OpenPGP data using IO Streams.
        /// </summary>
        /// <param name="inputStream">Input stream data containing the data to encrypt.</param>
        /// <param name="outputStream">Output stream which will contain encrypted data.</param>
        /// <param name="metadataStream">Output stream which will contain GPG metadata about the operation.</param>        
        /// <remarks>
        /// You must add the recipient's public key to your GnuPG key ring before calling this method.  Please see the GnuPG documentation for more information.
        /// </remarks>
        public void EncryptAsync(Stream inputStream, Stream outputStream, Stream metadataStream)
        {
          if (_asyncWorker != null && _asyncWorker.IsBusy)
              throw new InvalidOperationException("The GnuPG object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

          CreateAsyncWorker();
          _asyncWorker.WorkerSupportsCancellation = true;
          _asyncWorker.DoWork += new DoWorkEventHandler(EncryptAsync_DoWork);
          _asyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(EncryptAsync_RunWorkerCompleted);
          Object[] args = new Object[2];
          args[0] = inputStream;
          args[1] = outputStream;
          args[2] = metadataStream;
          _asyncWorker.RunWorkerAsync(args);
      }

        private void EncryptAsync_DoWork(object sender, DoWorkEventArgs e)
        {
          try
          {
              Object[] args = (Object[])e.Argument;
              Encrypt((Stream)args[0], (Stream)args[1], (Stream)args[2]);
          }
          catch (Exception ex)
          {
              _asyncException = ex;
          }
        }

        private void EncryptAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (EncryptAsyncCompleted != null)
                EncryptAsyncCompleted(this, new EncryptAsyncCompletedEventArgs(_asyncException, _asyncCancelled));
        }

        /// <summary>
        /// Event handler for DecryptAsync completed.
        /// </summary>
        public event EventHandler<DecryptAsyncCompletedEventArgs> DecryptAsyncCompleted;

        /// <summary>
        /// Starts asynchronous execution to decrypt OpenPGP data using IO Streams.
        /// </summary>
        /// <param name="inputStream">Input stream containing encrypted data.</param>
        /// <param name="outputStream">Output stream which will contain decrypted data.</param>
        /// <param name="metadataStream">Output stream which will contain GPG metadata about the operation.</param>        
        public void DecryptAsync(Stream inputStream, Stream outputStream, Stream metadataStream)
        {
            if (_asyncWorker != null && _asyncWorker.IsBusy)
                throw new InvalidOperationException("The GnuPG object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

            CreateAsyncWorker();
            _asyncWorker.WorkerSupportsCancellation = true;
            _asyncWorker.DoWork += new DoWorkEventHandler(DecryptAsync_DoWork);
            _asyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DecryptAsync_RunWorkerCompleted);
            Object[] args = new Object[2];
            args[0] = inputStream;
            args[1] = outputStream;
            args[2] = metadataStream;
            _asyncWorker.RunWorkerAsync(args);
        }

        private void DecryptAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Object[] args = (Object[])e.Argument;
                Decrypt((Stream)args[0], (Stream)args[1], (Stream)args[2]);
            }
            catch (Exception ex)
            {
                _asyncException = ex;
            }
        }

        private void DecryptAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (DecryptAsyncCompleted != null)
                DecryptAsyncCompleted(this, new DecryptAsyncCompletedEventArgs(_asyncException, _asyncCancelled));
        }

        /// <summary>
        /// Event handler for SignAsync completed.
        /// </summary>
        public event EventHandler<SignAsyncCompletedEventArgs> SignAsyncCompleted;

        /// <summary>
        /// Starts asynchronous execution to Sign OpenPGP data using IO Streams.
        /// </summary>
        /// <param name="inputStream">Input stream containing data to sign.</param>
        /// <param name="outputStream">Output stream which will contain Signed data.</param>
        /// <param name="metadataStream">Output stream which will contain GPG metadata about the operation.</param>        
        public void SignAsync(Stream inputStream, Stream outputStream, Stream metadataStream)
        {
            if (_asyncWorker != null && _asyncWorker.IsBusy)
                throw new InvalidOperationException("The GnuPG object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

            CreateAsyncWorker();
            _asyncWorker.WorkerSupportsCancellation = true;
            _asyncWorker.DoWork += new DoWorkEventHandler(SignAsync_DoWork);
            _asyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SignAsync_RunWorkerCompleted);
            Object[] args = new Object[2];
            args[0] = inputStream;
            args[1] = outputStream;
            args[2] = metadataStream;
            _asyncWorker.RunWorkerAsync(args);
        }

        private void SignAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Object[] args = (Object[])e.Argument;
                Sign((Stream)args[0], (Stream)args[1], (Stream)args[2]);
            }
            catch (Exception ex)
            {
                _asyncException = ex;
            }
        }

        private void SignAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (SignAsyncCompleted != null)
                SignAsyncCompleted(this, new SignAsyncCompletedEventArgs(_asyncException, _asyncCancelled));
        }





#endregion

  }
}
