﻿using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Axeno.Helper
{
    internal class CertificateManager
    {
        public static X509Certificate2 CreateCertificateAuthority(string caName, int keyStrength)
        {
            var random = new SecureRandom(new CryptoApiRandomGenerator());
            var keyPairGen = new RsaKeyPairGenerator();
            keyPairGen.Init(new KeyGenerationParameters(random, keyStrength));
            AsymmetricCipherKeyPair keypair = keyPairGen.GenerateKeyPair();

            var certificateGenerator = new X509V3CertificateGenerator();

            var CN = new X509Name("CN=" + caName);
            var SN = BigInteger.ProbablePrime(120, random);

            certificateGenerator.SetSerialNumber(SN);
            certificateGenerator.SetSubjectDN(CN);
            certificateGenerator.SetIssuerDN(CN);
            certificateGenerator.SetNotAfter(DateTime.MaxValue);
            certificateGenerator.SetNotBefore(DateTime.UtcNow.Subtract(new TimeSpan(2, 0, 0, 0)));
            certificateGenerator.SetPublicKey(keypair.Public);
            certificateGenerator.AddExtension(X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifierStructure(keypair.Public));
            certificateGenerator.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(true));

            ISignatureFactory signatureFactory = new Asn1SignatureFactory("SHA512WITHRSA", keypair.Private, random);

            var certificate = certificateGenerator.Generate(signatureFactory);

            var certificate2 = new X509Certificate2(DotNetUtilities.ToX509Certificate(certificate));
            certificate2.PrivateKey = DotNetUtilities.ToRSA(keypair.Private as RsaPrivateCrtKeyParameters);

            return certificate2;
        }
    }
}
