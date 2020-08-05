using System;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace BlazorDemo.Common.Utils
{
    public static class CryptoUtils
    {
        public static byte[] CreateCamelliaKey()
        {
            var random = new SecureRandom();
            var genParam = new KeyGenerationParameters(random, 256);
            var kGen = GeneratorUtilities.GetKeyGenerator("CAMELLIA");
            kGen.Init(genParam);
            return kGen.GenerateKey(); ;
        }

        public static byte[] EncryptCamellia(this byte[] data, byte[] key)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            var cipher = CipherUtilities.GetCipher("CAMELLIA");
            cipher.Init(true, ParameterUtilities.CreateKeyParameter("CAMELLIA", key));
            return cipher.DoFinal(data);
        }

        public static byte[] DecryptCamellia(this byte[] data, byte[] key)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var cipher = CipherUtilities.GetCipher("CAMELLIA");
            cipher.Init(false, ParameterUtilities.CreateKeyParameter("CAMELLIA", key));
            return cipher.DoFinal(data);
        }

        public static AsymmetricCipherKeyPair GenerateECKeyPair()
        {
            var curve = ECNamedCurveTable.GetByName("secp256k1");
            var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
            var sr = new SecureRandom();
            var keyParams = new ECKeyGenerationParameters(domainParams, sr);
            var generator = new ECKeyPairGenerator("ECDSA");
            generator.Init(keyParams);
            return generator.GenerateKeyPair();
        }

        public static AsymmetricCipherKeyPair CreateECKeyPair(byte[] privKey)
        {
            var curve = ECNamedCurveTable.GetByName("secp256k1");
            var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
            var d = new BigInteger(1, privKey);
            var ecPrivKey = new ECPrivateKeyParameters(d, domainParams);
            var q = domainParams.G.Multiply(d);
            var ecPubKey = new ECPublicKeyParameters(q, domainParams);
            return new AsymmetricCipherKeyPair(ecPubKey, ecPrivKey);
        }

        public static byte[] SignECDSA(this byte[] data, AsymmetricKeyParameter privKey)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            var signer = SignerUtilities.GetSigner("SHA256withECDSA");
            signer.Init(true, privKey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.GenerateSignature();
        }

        public static bool VerifyECDSA(this byte[] data, byte[] signature, AsymmetricKeyParameter pubKey)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var ecdsaVerify = SignerUtilities.GetSigner("SHA256withECDSA");
            ecdsaVerify.Init(false, pubKey);
            ecdsaVerify.BlockUpdate(data, 0, data.Length);
            return ecdsaVerify.VerifySignature(signature);
        }

        public static byte[] Sha3(this byte[] value)
        {

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var digest = new KeccakDigest(256);
            var output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(value, 0, value.Length);
            digest.DoFinal(output, 0);
            return output;
        }

        public static byte[] Sha3(this IEnumerable<byte> value) => value.ToArray().Sha3();

    }
}
