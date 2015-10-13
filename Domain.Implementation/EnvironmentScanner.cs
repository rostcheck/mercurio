using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercurio.Domain;
using System.Reflection;
using Cryptography.GPG;
using System.IO;
using System.Diagnostics;

namespace Mercurio.Domain.Implementation
{
    /// <summary>
    /// Scans the operating environment to find the available cryptographic providers, storage substrates, and similar resources
    /// </summary>
    public class EnvironmentScanner : IEnvironmentScanner
    {
        private List<ICryptographicServiceProvider> possibleCryptoProviderList;
        private ITempStorageSubstrate _tempDiskStorageSubstrate;

        public EnvironmentScanner(string userHome = null)
        {
            CryptoManagerConfiguration configuration = null;
            if (userHome != null)
            {
                configuration = new CryptoManagerConfiguration();
                configuration.Add(CryptoConfigurationKeyEnum.KeyringPath.ToString(), userHome);
            }
            possibleCryptoProviderList = new List<ICryptographicServiceProvider>();
            // Manually maintained list of things we know how to find
            RegisterCryptographicProvider(typeof(CrypographicServiceProviderGPG), configuration);
        }

        public List<ICryptographicServiceProvider> GetCryptographicProviders()
        {
            var availableProviders = new List<ICryptographicServiceProvider>();

            foreach (var provider in possibleCryptoProviderList)
            {
                if (provider.IsInstalled(GetOsAbstractor()))
                {
                    availableProviders.Add(provider);
                }
            }
            return availableProviders;
        }

        private void RegisterCryptographicProvider(Type type, CryptoManagerConfiguration configuration = null)
        {
            CryptoManagerFactory.Register(type.Name, type, configuration);
            possibleCryptoProviderList.Add((ICryptographicServiceProvider)Activator.CreateInstance(type));
        }

        public List<IStorageSubstrate> GetStorageSubstrates()
        {
            var returnList = new List<IStorageSubstrate>();
            var storageSubstratePath = ConfigurationManager.GetConfigurationValue("StorageSubstrate");
            if (storageSubstratePath != null)
            {
                if (!Directory.Exists(storageSubstratePath))
                    Directory.CreateDirectory(storageSubstratePath);
                returnList.Add(DiskStorageSubstrate.Create(storageSubstratePath, SerializerType.BinarySerializer));
            }
            return returnList;
        }

        public ITempStorageSubstrate GetTemporaryStorageSubstrate()
        {
            if (_tempDiskStorageSubstrate == null)
            {
                var tempStorageSubstratePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Guid.NewGuid().ToString()).ToString();
                Directory.CreateDirectory(tempStorageSubstratePath);
                _tempDiskStorageSubstrate = DiskTempStorageSubstrate.Create(tempStorageSubstratePath);
            }
            return _tempDiskStorageSubstrate;
        }

        public string GetEditor()
        {
            var osType = GetOsType();

            switch (osType)
            {
                case OSType.Windows:
                    // hard-coded to Notepad for now
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows).ToString(), "Notepad.exe").ToString();  
                case OSType.Mac:
                case OSType.Linux:
                    var editor = Environment.GetEnvironmentVariable("EDITOR");
                    if (string.IsNullOrEmpty(editor))
                        throw new MercurioException("EDITOR environment variable must be set");
                    return editor;
                default:
                    throw new MercurioException("OS Type not set when getting editor");
            }
        }

        public OSType GetOsType()
        {
            if (Path.DirectorySeparatorChar == '\\')
                return OSType.Windows;
            var unameOutput = ReadProcessOutput("uname");
            if (unameOutput.Contains("Darwin"))
                return OSType.Mac;
            else if (unameOutput.Contains("Linux"))
                return OSType.Linux;
            else return OSType.Unknown;
        }

        private IOSAbstractor GetOsAbstractor()
        {
            switch(GetOsType())
            {
                case OSType.Windows:
                    return new WindowsOsAbstractor();
                case OSType.Linux:
                    return new LinuxOsAbstractor();
                case OSType.Mac:
                    return new MacOsAbstractor();
                default:
                    throw new MercurioException("System OS type cannot be recognized");
            }
        }

        private static string ReadProcessOutput(string name)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = name;
                p.Start();

                // Read the output stream first and then wait.
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                if (output == null) output = "";
                output = output.Trim();
                return output;
            }
            catch
            {
                return "";
            }
        }
    }
}
