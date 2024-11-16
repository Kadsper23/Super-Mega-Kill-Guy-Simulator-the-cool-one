using System.IO;
using UnityEngine;

namespace Mirror.SimpleWeb
{
    internal class SslConfigLoader
    {
        internal struct Cert
        {
            public string path;
            public string password;
        }
        internal static SslConfig Load(SimpleWebTransport transport)
        {
            // don't need to load anything if ssl is not enabled
            if (!transport.sslEnabled)
                return default;

            string certJsonPath = transport.sslCertJson;

            Cert cert = LoadCertJson(certJsonPath);

            return new SslConfig(
                enabled: transport.sslEnabled,
                sslProtocols: transport.sslProtocols,
                certPath: cert.path,
                certPassword: cert.password
            );
        }

        internal static Cert LoadCertJson(string certJsonPath)
        {
            string json = File.ReadAllText(certJsonPath);
            Cert cert = JsonUtility.FromJson<Cert>(json);

<<<<<<<< Updated upstream:Assets/Mirror/Runtime/Transport/SimpleWebTransport/SslConfigLoader.cs
            if (string.IsNullOrEmpty(cert.path))
            {
                throw new InvalidDataException("Cert Json didn't not contain \"path\"");
            }
========
            if (string.IsNullOrWhiteSpace(cert.path))
            {
                throw new InvalidDataException("Cert Json didn't not contain \"path\"");
            }
            // don't use IsNullOrWhiteSpace here because whitespace could be a valid password for a cert
>>>>>>>> Stashed changes:Assets/Mirror/Runtime/Transports/SimpleWebTransport/SslConfigLoader.cs
            if (string.IsNullOrEmpty(cert.password))
            {
                // password can be empty
                cert.password = string.Empty;
            }

            return cert;
        }
    }
}
