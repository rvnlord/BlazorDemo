using System;
using System.Linq;
using BlazorDemo.Common.Utils;
using MoreLinq;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace BlazorDemo.Common.Converters
{
    public static class CryptoConverter
    {
        public static byte[] ToECPrivateKeyByteArray(this AsymmetricKeyParameter ecPrivateKey)
        {
            if (ecPrivateKey == null)
                throw new ArgumentNullException(nameof(ecPrivateKey));

            return ((ECPrivateKeyParameters)ecPrivateKey).D.ToString(16).HexToByteArray().PadStart<byte>(32, 0x00).ToArray();
        }

        public static AsymmetricKeyParameter ECPrivateKeyByteArrayToECPrivateKey(this byte[] ecPrivateKey)
        {
            return CryptoUtils.CreateECKeyPair(ecPrivateKey).Private;
        }

        public static byte[] ECPrivateKeyByteArrayToECPublicKeyByteArray(this byte[] ecPrivateKey)
        {
            return CryptoUtils.CreateECKeyPair(ecPrivateKey).Public.ToECPublicKeyByteArray();
        }

        public static byte[] ToECPublicKeyByteArray(this AsymmetricKeyParameter ecPublicKey)
        {
            if (ecPublicKey == null)
                throw new ArgumentNullException(nameof(ecPublicKey));

            var publicKey = ((ECPublicKeyParameters)ecPublicKey).Q;
            var xs = publicKey.AffineXCoord.ToBigInteger().ToByteArrayUnsigned().PadStart(32);
            var ys = publicKey.AffineYCoord.ToBigInteger().ToByteArrayUnsigned().PadStart(32);
            return xs.Concat(ys).ToArray();
        }

        public static AsymmetricKeyParameter ECPublicKeyByteArrayToECPublicKey(this byte[] ecPublicKey)
        {
            var curve = ECNamedCurveTable.GetByName("secp256k1");
            var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
            var x = new BigInteger(1, ecPublicKey.Take(32).ToArray());
            var y = new BigInteger(1, ecPublicKey.Skip(32).ToArray());
            var q = curve.Curve.CreatePoint(x, y);
            return new ECPublicKeyParameters(q, domainParams);
        }
    }
}
