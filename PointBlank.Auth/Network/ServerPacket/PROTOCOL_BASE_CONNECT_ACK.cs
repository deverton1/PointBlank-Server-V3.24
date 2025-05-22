﻿// Decompiled with JetBrains decompiler
// Type: PointBlank.Auth.Network.ServerPacket.PROTOCOL_BASE_CONNECT_ACK
// Assembly: PointBlank.Auth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2F2D71E3-3E87-4155-AA64-E654DA3CFF2D
// Assembly location: C:\Users\LucasRoot\Desktop\Servidor BG\PointBlank.Auth.exe

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using PointBlank.Core;
using PointBlank.Core.Network;
using System;
using System.Net;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_CONNECT_ACK : SendPacket
  {
    private IPAddress Ip;
    private uint SessionId;
    private ushort SessionSeed;

    public PROTOCOL_BASE_CONNECT_ACK(AuthClient Client)
    {
      this.SessionId = Client.SessionId;
      this.SessionSeed = Client.SessionSeed;
      this.Ip = Client.GetAddress();
    }

    public override void write()
    {
      byte[] derEncoded = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(PROTOCOL_BASE_CONNECT_ACK.GeneratePair().Public).ToAsn1Object().GetDerEncoded();
      byte[] numArray1 = new byte[128];
      Buffer.BlockCopy((Array) derEncoded, 29, (Array) numArray1, 0, numArray1.Length);
      byte[] numArray2 = new byte[3]
      {
        (byte) 1,
        (byte) 0,
        (byte) 17
      };
      short num1 = 2;
      int num2 = numArray1.Length + numArray2.Length + (int) num1;
      this.CheckIp(this.Ip);
      this.writeH((short) 514);
      this.writeH((short) 2);
      this.writeC((byte) 10);
      for (int index = 0; index < 10; ++index)
        this.writeC((byte) 0);
      this.writeH((short) num2);
      this.writeH((short) numArray1.Length);
      this.writeB(numArray1);
      this.writeB(numArray2);
      this.writeC((byte) 3);
      this.writeH((short) 24);
      this.writeH(this.SessionSeed);
      this.writeD(this.SessionId);
    }

    public void CheckIp(IPAddress Ip) => Logger.LogProblems(Ip.ToString(), "Ip/Auth");

    public static AsymmetricCipherKeyPair GeneratePair()
    {
      SecureRandom random = new SecureRandom((IRandomGenerator) new CryptoApiRandomGenerator());
      RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
      keyPairGenerator.Init(new KeyGenerationParameters(random, 1024));
      return keyPairGenerator.GenerateKeyPair();
    }
  }
}
