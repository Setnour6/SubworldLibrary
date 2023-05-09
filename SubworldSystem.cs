using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Events;
using Terraria.Graphics.Capture;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Net;
using Terraria.Net.Sockets;
using Terraria.Social;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace SubworldLibrary;

public class SubworldSystem : ModSystem
{
	internal static List<Subworld> subworlds;

	internal static Subworld current;

	internal static Subworld cache;

	internal static WorldFileData main;

	internal static TagCompound copiedData;

	internal static Dictionary<RemoteAddress, int> playerLocations;

	internal static Dictionary<int, NamedPipeClientStream> pipes;

	public static bool noReturn;

	public static bool hideUnderworld;

	public static Subworld Current => SubworldSystem.current;

	public static string CurrentPath => Path.Combine(Main.WorldPath, Path.GetFileNameWithoutExtension(((FileData)SubworldSystem.main).get_Path()), ((ModType)SubworldSystem.current).get_Mod().get_Name() + "_" + ((ModType)SubworldSystem.current).get_Name() + ".wld");

	public override void OnModLoad()
	{
		SubworldSystem.subworlds = new List<Subworld>();
		Hooks.add_OnEnterWorld((Action<Player>)OnEnterWorld);
		Netplay.add_OnDisconnect((Action)OnDisconnect);
	}

	public override void Unload()
	{
		Hooks.remove_OnEnterWorld((Action<Player>)OnEnterWorld);
		Netplay.remove_OnDisconnect((Action)OnDisconnect);
	}

	public static bool IsActive(string id)
	{
		Subworld subworld = SubworldSystem.current;
		return ((subworld != null) ? ((ModType)subworld).get_FullName() : null) == id;
	}

	public static bool IsActive<T>() where T : Subworld
	{
		return ((object)SubworldSystem.current)?.GetType() == typeof(T);
	}

	public static bool AnyActive(Mod mod)
	{
		Subworld subworld = SubworldSystem.current;
		return ((subworld != null) ? ((ModType)subworld).get_Mod() : null) == mod;
	}

	public static bool AnyActive<T>() where T : Mod
	{
		Subworld subworld = SubworldSystem.current;
		return ((subworld != null) ? ((ModType)subworld).get_Mod() : null) == (object)ModContent.GetInstance<T>();
	}

	public static bool Enter(string id)
	{
		if (SubworldSystem.current == SubworldSystem.cache)
		{
			for (int i = 0; i < SubworldSystem.subworlds.Count; i++)
			{
				if (((ModType)SubworldSystem.subworlds[i]).get_FullName() == id)
				{
					SubworldSystem.BeginEntering(i);
					return true;
				}
			}
		}
		return false;
	}

	public static bool Enter<T>() where T : Subworld
	{
		if (SubworldSystem.current == SubworldSystem.cache)
		{
			for (int i = 0; i < SubworldSystem.subworlds.Count; i++)
			{
				if (((object)SubworldSystem.subworlds[i]).GetType() == typeof(T))
				{
					SubworldSystem.BeginEntering(i);
					return true;
				}
			}
		}
		return false;
	}

	public static void Exit()
	{
		if (SubworldSystem.current != null && SubworldSystem.current == SubworldSystem.cache)
		{
			if (Main.netMode == 0)
			{
				Task.Factory.StartNew(ExitWorldCallBack, null);
			}
			else if (Main.netMode == 1)
			{
				ModPacket packet = ((Mod)ModContent.GetInstance<SubworldLibrary>()).GetPacket(256);
				((BinaryWriter)packet).Write((byte)1);
				packet.Send(-1, -1);
			}
		}
	}

	public static void CopyWorldData(string key, object data)
	{
		if (data != null && !SubworldSystem.copiedData.ContainsKey(key))
		{
			SubworldSystem.copiedData.set_Item(key, data);
		}
	}

	public static T ReadCopiedWorldData<T>(string key)
	{
		return SubworldSystem.copiedData.Get<T>(key);
	}

	public static void SendToMainServer(Mod mod, byte[] data)
	{
		int extra = ((ModNet.get_NetModCount() < 256) ? 6 : 8);
		byte[] packet = new byte[data.Length + extra];
		int i = 0;
		packet[i++] = 0;
		packet[i++] = (byte)packet.Length;
		packet[i++] = (byte)(packet.Length >> 8);
		packet[i++] = 250;
		short netID = ((Mod)ModContent.GetInstance<SubworldLibrary>()).get_NetID();
		packet[i++] = (byte)netID;
		if (ModNet.get_NetModCount() >= 256)
		{
			packet[i++] = (byte)(netID >> 8);
			packet[i + 1] = (byte)(mod.get_NetID() >> 8);
		}
		packet[i++] = (byte)mod.get_NetID();
		Buffer.BlockCopy(data, 0, packet, extra, data.Length);
		SubworldSystem.SendToPipe(SubserverSocket.pipe, packet);
	}

	private static void GenerateSubworlds()
	{
		SubworldSystem.main = Main.ActiveWorldFileData;
		bool cloud = ((FileData)SubworldSystem.main).get_IsCloudSave();
		foreach (Subworld subworld in SubworldSystem.subworlds)
		{
			if (subworld.ShouldSave)
			{
				SubworldSystem.current = subworld;
				SubworldSystem.LoadSubworld(SubworldSystem.CurrentPath, cloud);
				WorldFile.SaveWorld(cloud, false);
				Main.ActiveWorldFileData = SubworldSystem.main;
			}
		}
	}

	private static void EraseSubworlds(int index)
	{
		WorldFileData world = Main.WorldList[index];
		string path = Path.Combine(Main.WorldPath, Path.GetFileNameWithoutExtension(((FileData)world).get_Path()));
		if (FileUtilities.Exists(path, ((FileData)world).get_IsCloudSave()))
		{
			FileUtilities.Delete(path, ((FileData)world).get_IsCloudSave(), false);
		}
	}

	private static void BeginEntering(int index)
	{
		if (Main.netMode == 0)
		{
			if (SubworldSystem.current == null)
			{
				SubworldSystem.main = Main.ActiveWorldFileData;
			}
			Task.Factory.StartNew(ExitWorldCallBack, SubworldSystem.subworlds[index]);
		}
		else if (Main.netMode == 1)
		{
			ModPacket packet = ((Mod)ModContent.GetInstance<SubworldLibrary>()).GetPacket(256);
			((BinaryWriter)packet).Write((byte)0);
			((BinaryWriter)packet).Write((ushort)index);
			packet.Send(-1, -1);
		}
	}

	private static void CopyMainWorldData()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		SubworldSystem.copiedData.set_Item("seed", (object)Main.ActiveWorldFileData.get_SeedText());
		SubworldSystem.copiedData.set_Item("gameMode", (object)Main.ActiveWorldFileData.GameMode);
		SubworldSystem.copiedData.set_Item("hardMode", (object)Main.hardMode);
		MemoryStream bestiary = new MemoryStream();
		try
		{
			BinaryWriter writer = new BinaryWriter((Stream)(object)bestiary);
			try
			{
				Main.BestiaryTracker.Save(writer);
				SubworldSystem.copiedData.set_Item("bestiary", (object)bestiary.GetBuffer());
			}
			finally
			{
				((IDisposable)writer)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)bestiary)?.Dispose();
		}
		SubworldSystem.copiedData.set_Item("drunkWorld", (object)Main.drunkWorld);
		SubworldSystem.copiedData.set_Item("getGoodWorld", (object)Main.getGoodWorld);
		SubworldSystem.copiedData.set_Item("tenthAnniversaryWorld", (object)Main.tenthAnniversaryWorld);
		SubworldSystem.copiedData.set_Item("dontStarveWorld", (object)Main.dontStarveWorld);
		SubworldSystem.copiedData.set_Item("notTheBeesWorld", (object)Main.notTheBeesWorld);
		SubworldSystem.CopyDowned();
		foreach (Subworld subworld in SubworldSystem.subworlds)
		{
			subworld.CopyMainWorldData();
		}
	}

	private static void ReadCopiedMainWorldData()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		Main.ActiveWorldFileData.SetSeed(SubworldSystem.copiedData.Get<string>("seed"));
		Main.set_GameMode(SubworldSystem.copiedData.Get<int>("gameMode"));
		Main.hardMode = SubworldSystem.copiedData.Get<bool>("hardMode");
		MemoryStream bestiary = new MemoryStream(SubworldSystem.copiedData.Get<byte[]>("bestiary"));
		try
		{
			BinaryReader reader = new BinaryReader((Stream)(object)bestiary);
			try
			{
				Main.BestiaryTracker.Load(reader, 248);
			}
			finally
			{
				((IDisposable)reader)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)bestiary)?.Dispose();
		}
		Main.drunkWorld = SubworldSystem.copiedData.Get<bool>("drunkWorld");
		Main.getGoodWorld = SubworldSystem.copiedData.Get<bool>("getGoodWorld");
		Main.tenthAnniversaryWorld = SubworldSystem.copiedData.Get<bool>("tenthAnniversaryWorld");
		Main.dontStarveWorld = SubworldSystem.copiedData.Get<bool>("dontStarveWorld");
		Main.notTheBeesWorld = SubworldSystem.copiedData.Get<bool>("notTheBeesWorld");
		SubworldSystem.ReadCopiedDowned();
		foreach (Subworld subworld in SubworldSystem.subworlds)
		{
			subworld.ReadCopiedMainWorldData();
		}
	}

	private static void CopyDowned()
	{
		SubworldSystem.copiedData.set_Item("downedSlimeKing", (object)NPC.downedSlimeKing);
		SubworldSystem.copiedData.set_Item("downedBoss1", (object)NPC.downedBoss1);
		SubworldSystem.copiedData.set_Item("downedBoss2", (object)NPC.downedBoss2);
		SubworldSystem.copiedData.set_Item("downedBoss3", (object)NPC.downedBoss3);
		SubworldSystem.copiedData.set_Item("downedQueenBee", (object)NPC.downedQueenBee);
		SubworldSystem.copiedData.set_Item("downedDeerclops", (object)NPC.downedDeerclops);
		SubworldSystem.copiedData.set_Item("downedQueenSlime", (object)NPC.downedQueenSlime);
		SubworldSystem.copiedData.set_Item("downedMechBoss1", (object)NPC.downedMechBoss1);
		SubworldSystem.copiedData.set_Item("downedMechBoss2", (object)NPC.downedMechBoss2);
		SubworldSystem.copiedData.set_Item("downedMechBoss3", (object)NPC.downedMechBoss3);
		SubworldSystem.copiedData.set_Item("downedMechBossAny", (object)NPC.downedMechBossAny);
		SubworldSystem.copiedData.set_Item("downedPlantBoss", (object)NPC.downedPlantBoss);
		SubworldSystem.copiedData.set_Item("downedGolemBoss", (object)NPC.downedGolemBoss);
		SubworldSystem.copiedData.set_Item("downedFishron", (object)NPC.downedFishron);
		SubworldSystem.copiedData.set_Item("downedEmpressOfLight", (object)NPC.downedEmpressOfLight);
		SubworldSystem.copiedData.set_Item("downedAncientCultist", (object)NPC.downedAncientCultist);
		SubworldSystem.copiedData.set_Item("downedTowerSolar", (object)NPC.downedTowerSolar);
		SubworldSystem.copiedData.set_Item("downedTowerVortex", (object)NPC.downedTowerVortex);
		SubworldSystem.copiedData.set_Item("downedTowerNebula", (object)NPC.downedTowerNebula);
		SubworldSystem.copiedData.set_Item("downedTowerStardust", (object)NPC.downedTowerStardust);
		SubworldSystem.copiedData.set_Item("downedMoonlord", (object)NPC.downedMoonlord);
		SubworldSystem.copiedData.set_Item("downedGoblins", (object)NPC.downedGoblins);
		SubworldSystem.copiedData.set_Item("downedClown", (object)NPC.downedClown);
		SubworldSystem.copiedData.set_Item("downedFrost", (object)NPC.downedFrost);
		SubworldSystem.copiedData.set_Item("downedPirates", (object)NPC.downedPirates);
		SubworldSystem.copiedData.set_Item("downedMartians", (object)NPC.downedMartians);
		SubworldSystem.copiedData.set_Item("downedHalloweenTree", (object)NPC.downedHalloweenTree);
		SubworldSystem.copiedData.set_Item("downedHalloweenKing", (object)NPC.downedHalloweenKing);
		SubworldSystem.copiedData.set_Item("downedChristmasTree", (object)NPC.downedChristmasTree);
		SubworldSystem.copiedData.set_Item("downedChristmasSantank", (object)NPC.downedChristmasSantank);
		SubworldSystem.copiedData.set_Item("downedChristmasIceQueen", (object)NPC.downedChristmasIceQueen);
		SubworldSystem.copiedData.set_Item("DownedInvasionT1", (object)DD2Event.DownedInvasionT1);
		SubworldSystem.copiedData.set_Item("DownedInvasionT2", (object)DD2Event.DownedInvasionT2);
		SubworldSystem.copiedData.set_Item("DownedInvasionT3", (object)DD2Event.DownedInvasionT3);
	}

	private static void ReadCopiedDowned()
	{
		NPC.downedSlimeKing = SubworldSystem.copiedData.Get<bool>("downedSlimeKing");
		NPC.downedBoss1 = SubworldSystem.copiedData.Get<bool>("downedBoss1");
		NPC.downedBoss2 = SubworldSystem.copiedData.Get<bool>("downedBoss2");
		NPC.downedBoss3 = SubworldSystem.copiedData.Get<bool>("downedBoss3");
		NPC.downedQueenBee = SubworldSystem.copiedData.Get<bool>("downedQueenBee");
		NPC.downedDeerclops = SubworldSystem.copiedData.Get<bool>("downedDeerclops");
		NPC.downedQueenSlime = SubworldSystem.copiedData.Get<bool>("downedQueenSlime");
		NPC.downedMechBoss1 = SubworldSystem.copiedData.Get<bool>("downedMechBoss1");
		NPC.downedMechBoss2 = SubworldSystem.copiedData.Get<bool>("downedMechBoss2");
		NPC.downedMechBoss3 = SubworldSystem.copiedData.Get<bool>("downedMechBoss3");
		NPC.downedMechBossAny = SubworldSystem.copiedData.Get<bool>("downedMechBossAny");
		NPC.downedPlantBoss = SubworldSystem.copiedData.Get<bool>("downedPlantBoss");
		NPC.downedGolemBoss = SubworldSystem.copiedData.Get<bool>("downedGolemBoss");
		NPC.downedFishron = SubworldSystem.copiedData.Get<bool>("downedFishron");
		NPC.downedEmpressOfLight = SubworldSystem.copiedData.Get<bool>("downedEmpressOfLight");
		NPC.downedAncientCultist = SubworldSystem.copiedData.Get<bool>("downedAncientCultist");
		NPC.downedTowerSolar = SubworldSystem.copiedData.Get<bool>("downedTowerSolar");
		NPC.downedTowerVortex = SubworldSystem.copiedData.Get<bool>("downedTowerVortex");
		NPC.downedTowerNebula = SubworldSystem.copiedData.Get<bool>("downedTowerNebula");
		NPC.downedTowerStardust = SubworldSystem.copiedData.Get<bool>("downedTowerStardust");
		NPC.downedMoonlord = SubworldSystem.copiedData.Get<bool>("downedMoonlord");
		NPC.downedGoblins = SubworldSystem.copiedData.Get<bool>("downedGoblins");
		NPC.downedClown = SubworldSystem.copiedData.Get<bool>("downedClown");
		NPC.downedFrost = SubworldSystem.copiedData.Get<bool>("downedFrost");
		NPC.downedPirates = SubworldSystem.copiedData.Get<bool>("downedPirates");
		NPC.downedMartians = SubworldSystem.copiedData.Get<bool>("downedMartians");
		NPC.downedHalloweenTree = SubworldSystem.copiedData.Get<bool>("downedHalloweenTree");
		NPC.downedHalloweenKing = SubworldSystem.copiedData.Get<bool>("downedHalloweenKing");
		NPC.downedChristmasTree = SubworldSystem.copiedData.Get<bool>("downedChristmasTree");
		NPC.downedChristmasSantank = SubworldSystem.copiedData.Get<bool>("downedChristmasSantank");
		NPC.downedChristmasIceQueen = SubworldSystem.copiedData.Get<bool>("downedChristmasIceQueen");
		DD2Event.DownedInvasionT1 = SubworldSystem.copiedData.Get<bool>("DownedInvasionT1");
		DD2Event.DownedInvasionT2 = SubworldSystem.copiedData.Get<bool>("DownedInvasionT2");
		DD2Event.DownedInvasionT3 = SubworldSystem.copiedData.Get<bool>("DownedInvasionT3");
	}

	internal static void SendToPipe(NamedPipeClientStream pipe, byte[] data)
	{
		Task.Run(delegate
		{
			if (!pipe.IsConnected)
			{
				pipe.Connect(5000);
			}
			pipe.Write(data);
		});
	}

	public static void StartSubserver(int id)
	{
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		if (SubworldSystem.pipes.ContainsKey(id))
		{
			return;
		}
		Process p = new Process();
		p.StartInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
		p.StartInfo.Arguments = "tModLoader.dll -server -showserverconsole -world \"" + Main.get_worldPathName() + "\" -subworld \"" + ((ModType)SubworldSystem.subworlds[id]).get_FullName() + "\"";
		p.StartInfo.UseShellExecute = true;
		p.Start();
		SubworldSystem.pipes[id] = new NamedPipeClientStream(".", ((ModType)SubworldSystem.subworlds[id]).get_FullName() + "/IN", PipeDirection.Out);
		Thread thread = new Thread(MainServerCallBack);
		thread.Name = "Subserver Packet Thread";
		thread.IsBackground = true;
		thread.Start(id);
		SubworldSystem.copiedData = new TagCompound();
		SubworldSystem.CopyMainWorldData();
		MemoryStream stream = new MemoryStream();
		try
		{
			TagIO.ToStream(SubworldSystem.copiedData, (Stream)(object)stream, true);
			SubworldSystem.pipes[id].Connect(15000);
			SubworldSystem.pipes[id].Write(stream.ToArray());
			SubworldSystem.copiedData = null;
		}
		finally
		{
			((IDisposable)stream)?.Dispose();
		}
	}

	private static void MainServerCallBack(object id)
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		NamedPipeServerStream pipe = new NamedPipeServerStream(((ModType)SubworldSystem.subworlds[(int)id]).get_FullName() + "/OUT", PipeDirection.In, -1);
		try
		{
			pipe.WaitForConnection();
			while (pipe.IsConnected)
			{
				byte[] packetInfo = new byte[3];
				if (pipe.Read(packetInfo) < 3)
				{
					break;
				}
				byte low = packetInfo[1];
				byte high = packetInfo[2];
				int length = (high << 8) | low;
				byte[] data = new byte[length];
				pipe.Read(data, 2, length - 2);
				data[0] = low;
				data[1] = high;
				if (data[2] == 250 && ((ModNet.get_NetModCount() < 256) ? data[3] : BitConverter.ToUInt16(data, 3)) == ((Mod)ModContent.GetInstance<SubworldLibrary>()).get_NetID())
				{
					MemoryStream stream = new MemoryStream(data);
					BinaryReader reader = new BinaryReader((Stream)(object)stream);
					try
					{
						if (ModNet.get_NetModCount() < 256)
						{
							((Stream)(object)stream).Position = 5L;
							ModNet.GetMod((int)data[4]).HandlePacket(reader, 256);
						}
						else
						{
							((Stream)(object)stream).Position = 6L;
							ModNet.GetMod((int)BitConverter.ToUInt16(data, 5)).HandlePacket(reader, 256);
						}
					}
					finally
					{
						((IDisposable)reader)?.Dispose();
					}
				}
				else
				{
					ISocket socket = Netplay.Clients[packetInfo[0]].Socket;
					object obj = _003C_003Ec._003C_003E9__34_0;
					if (obj == null)
					{
						SocketSendCallback val = delegate
						{
						};
						obj = (object)val;
						_003C_003Ec._003C_003E9__34_0 = val;
					}
					socket.AsyncSend(data, 0, length, (SocketSendCallback)obj, (object)null);
				}
			}
		}
		finally
		{
			pipe.Close();
			SubworldSystem.pipes[(int)id].Close();
			SubworldSystem.pipes.Remove((int)id);
		}
	}

	private static bool LoadIntoSubworld()
	{
		if (Program.LaunchParameters.TryGetValue("-subworld", out var id))
		{
			for (int i = 0; i < SubworldSystem.subworlds.Count; i++)
			{
				if (((ModType)SubworldSystem.subworlds[i]).get_FullName() == id)
				{
					Main.myPlayer = 255;
					SubworldSystem.main = Main.ActiveWorldFileData;
					SubworldSystem.current = SubworldSystem.subworlds[i];
					NamedPipeServerStream pipe = new NamedPipeServerStream(((ModType)SubworldSystem.current).get_FullName() + "/IN", PipeDirection.In, -1);
					pipe.WaitForConnection();
					SubworldSystem.copiedData = TagIO.FromStream((Stream)pipe, true);
					SubworldSystem.LoadWorld();
					SubworldSystem.copiedData = null;
					for (int j = 0; j < Netplay.Clients.Length; j++)
					{
						Netplay.Clients[j].Id = j;
						Netplay.Clients[j].Reset();
						Netplay.Clients[j].ReadBuffer = null;
					}
					SubserverSocket.pipe = new NamedPipeClientStream(".", ((ModType)SubworldSystem.current).get_FullName() + "/OUT", PipeDirection.Out);
					Thread thread = new Thread(SubserverCallBack);
					thread.IsBackground = true;
					thread.Start(pipe);
					return true;
				}
			}
			((Game)Main.instance).Exit();
			return true;
		}
		SubworldSystem.playerLocations = new Dictionary<RemoteAddress, int>();
		SubworldSystem.pipes = new Dictionary<int, NamedPipeClientStream>();
		return false;
	}

	private static void SubserverCallBack(object pipeObject)
	{
		NamedPipeServerStream pipe = (NamedPipeServerStream)pipeObject;
		try
		{
			while (!Netplay.Disconnect)
			{
				byte[] packetInfo = new byte[3];
				if (pipe.Read(packetInfo) < 3)
				{
					break;
				}
				MessageBuffer buffer = NetMessage.buffer[packetInfo[0]];
				int length = BitConverter.ToUInt16(packetInfo, 1);
				lock (buffer)
				{
					while (buffer.totalData + length > buffer.readBuffer.Length)
					{
						Monitor.Exit(buffer);
						Thread.Yield();
						Monitor.Enter(buffer);
					}
					buffer.readBuffer[buffer.totalData] = packetInfo[1];
					buffer.readBuffer[buffer.totalData + 1] = packetInfo[2];
					pipe.Read(buffer.readBuffer, buffer.totalData + 2, length - 2);
					if (buffer.readBuffer[buffer.totalData + 2] == 1)
					{
						Netplay.Clients[buffer.whoAmI].Socket = (ISocket)(object)new SubserverSocket(buffer.whoAmI);
						Netplay.Clients[buffer.whoAmI].IsActive = true;
						Netplay.HasClients = true;
					}
					buffer.totalData += length;
					buffer.checkBytes = true;
				}
			}
		}
		finally
		{
			pipe.Close();
			SubserverSocket.pipe.Close();
		}
	}

	internal static void ExitWorldCallBack(object subworldObject)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		int netMode = Main.netMode;
		if (netMode == 0)
		{
			WorldFile.CacheSaveTime();
			if (SubworldSystem.copiedData == null)
			{
				SubworldSystem.copiedData = new TagCompound();
			}
			if (SubworldSystem.cache != null)
			{
				SubworldSystem.cache.CopySubworldData();
				SubworldSystem.cache.OnExit();
			}
			if (subworldObject != null)
			{
				SubworldSystem.CopyMainWorldData();
			}
		}
		else
		{
			Netplay.Connection.State = 1;
			SubworldSystem.cache?.OnExit();
		}
		SubworldSystem.current = (Subworld)subworldObject;
		Main.invasionProgress = -1;
		Main.invasionProgressDisplayLeft = 0;
		Main.invasionProgressAlpha = 0f;
		Main.invasionProgressIcon = 0;
		SubworldSystem.noReturn = false;
		if (SubworldSystem.current != null)
		{
			SubworldSystem.hideUnderworld = true;
			SubworldSystem.current.OnEnter();
		}
		else
		{
			SubworldSystem.hideUnderworld = false;
		}
		Main.gameMenu = true;
		SoundEngine.StopTrackedSounds();
		CaptureInterface.ResetFocus();
		Main.ActivePlayerFileData.StopPlayTimer();
		Player.SavePlayer(Main.ActivePlayerFileData, false);
		Player.ClearPlayerTempInfo();
		Rain.ClearRain();
		if (netMode != 1)
		{
			WorldFile.SaveWorld();
		}
		SystemLoader.OnWorldUnload();
		Main.fastForwardTime/* tModPorter Note: Removed. Suggestion: IsFastForwardingTime(), fastForwardTimeToDawn or fastForwardTimeToDusk */ = false;
		Main.UpdateTimeRate();
		WorldGen.noMapUpdate = true;
		if (SubworldSystem.cache != null && SubworldSystem.cache.NoPlayerSaving)
		{
			PlayerFileData playerData = Player.GetFileData(((FileData)Main.ActivePlayerFileData).get_Path(), ((FileData)Main.ActivePlayerFileData).get_IsCloudSave());
			if (playerData != null)
			{
				((Entity)playerData.get_Player()).whoAmI = Main.myPlayer;
				((FileData)playerData).SetAsActive();
			}
		}
		if (netMode != 1)
		{
			SubworldSystem.LoadWorld();
		}
		else
		{
			NetMessage.SendData(1, -1, -1, (NetworkText)null, 0, 0f, 0f, 0f, 0, 0, 0);
		}
	}

	private static void LoadWorld()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		bool isSubworld = SubworldSystem.current != null;
		WorldGen.gen = true;
		WorldGen.loadFailed = false;
		WorldGen.loadSuccess = false;
		Main.set_rand(new UnifiedRandom((int)DateTime.Now.Ticks));
		bool cloud = ((FileData)SubworldSystem.main).get_IsCloudSave();
		string path = (isSubworld ? SubworldSystem.CurrentPath : ((FileData)SubworldSystem.main).get_Path());
		SubworldSystem.cache?.OnUnload();
		if (!isSubworld || SubworldSystem.current.ShouldSave)
		{
			if (!isSubworld)
			{
				Main.ActiveWorldFileData = SubworldSystem.main;
			}
			SubworldSystem.TryLoadWorldFile(path, cloud, 0);
		}
		if (isSubworld)
		{
			if (WorldGen.loadFailed)
			{
				((Mod)ModContent.GetInstance<SubworldLibrary>()).get_Logger().Warn((object)("Failed to load \"" + Main.worldName + (WorldGen.worldBackup ? "\" from file" : "\" from file, no backup")));
			}
			if (!WorldGen.loadSuccess)
			{
				SubworldSystem.LoadSubworld(path, cloud);
			}
			SubworldSystem.current.OnLoad();
		}
		else if (!WorldGen.loadSuccess)
		{
			((Mod)ModContent.GetInstance<SubworldLibrary>()).get_Logger().Error((object)("Failed to load \"" + ((FileData)SubworldSystem.main).Name + (WorldGen.worldBackup ? "\" from file" : "\" from file, no backup")));
			Main.menuMode = 0;
			if (Main.netMode == 2)
			{
				Netplay.Disconnect = true;
			}
			return;
		}
		WorldGen.gen = false;
		if (Main.netMode != 2)
		{
			if (Main.mapEnabled)
			{
				Main.Map.Load();
			}
			Main.sectionManager.SetAllFramesLoaded();
			while (Main.mapEnabled && Main.loadMapLock)
			{
				Main.set_statusText(Lang.gen[68].get_Value() + " " + (int)((float)Main.loadMapLastX / (float)Main.maxTilesX * 100f + 1f) + "%");
				Thread.Sleep(0);
			}
			Player player = Main.get_LocalPlayer();
			if (Main.anglerWhoFinishedToday.Contains(player.name))
			{
				Main.anglerQuestFinished = true;
			}
			player.Spawn((PlayerSpawnContext)1);
			Main.ActivePlayerFileData.StartPlayTimer();
			Hooks.EnterWorld(Main.myPlayer);
			WorldFile.SetOngoingToTemps();
			Main.resetClouds = true;
			Main.gameMenu = false;
		}
	}

	private static void OnEnterWorld(Player player)
	{
		if (Main.netMode == 1)
		{
			SubworldSystem.cache?.OnUnload();
			SubworldSystem.current?.OnLoad();
		}
		SubworldSystem.cache = SubworldSystem.current;
	}

	private static void OnDisconnect()
	{
		if (SubworldSystem.current != null || SubworldSystem.cache != null)
		{
			Main.menuMode = 14;
		}
		SubworldSystem.current = null;
		SubworldSystem.cache = null;
	}

	private static void LoadSubworld(string path, bool cloud)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected O, but got Unknown
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Expected O, but got Unknown
		Main.worldName = Language.GetTextValue("Mods." + ((ModType)SubworldSystem.current).get_Mod().get_Name() + ".SubworldName." + ((ModType)SubworldSystem.current).get_Name());
		if (Main.netMode == 2)
		{
			Console.Title = Main.worldName;
		}
		WorldFileData data = new WorldFileData(path, cloud)
		{
			Name = Main.worldName,
			GameMode = Main.get_GameMode(),
			CreationTime = DateTime.Now,
			Metadata = FileMetadata.FromCurrentSettings((FileType)2),
			WorldGeneratorVersion = 1065151889409uL,
			UniqueId = Guid.NewGuid()
		};
		data.SetSeed(SubworldSystem.main.get_SeedText());
		Main.ActiveWorldFileData = data;
		Main.maxTilesX = SubworldSystem.current.Width;
		Main.maxTilesY = SubworldSystem.current.Height;
		Main.spawnTileX = Main.maxTilesX / 2;
		Main.spawnTileY = Main.maxTilesY / 2;
		WorldGen.setWorldSize();
		WorldGen.clearWorld();
		Main.worldSurface = (double)Main.maxTilesY * 0.3;
		Main.rockLayer = (double)Main.maxTilesY * 0.5;
		GenVars.waterLine = Main.maxTilesY;
		Main.weatherCounter = 18000;
		Cloud.resetClouds();
		SubworldSystem.ReadCopiedMainWorldData();
		float weight = 0f;
		for (int j = 0; j < SubworldSystem.current.Tasks.Count; j++)
		{
			weight += SubworldSystem.current.Tasks[j].Weight;
		}
		WorldGenerator.CurrentGenerationProgress = new GenerationProgress();
		WorldGenerator.CurrentGenerationProgress.TotalWeight = weight;
		WorldGenConfiguration config = SubworldSystem.current.Config;
		for (int i = 0; i < SubworldSystem.current.Tasks.Count; i++)
		{
			WorldGen._genRand = new UnifiedRandom(data.get_Seed());
			Main.set_rand(new UnifiedRandom(data.get_Seed()));
			GenPass task = SubworldSystem.current.Tasks[i];
			WorldGenerator.CurrentGenerationProgress.Start(task.Weight);
			task.Apply(WorldGenerator.CurrentGenerationProgress, (config != null) ? config.GetPassConfiguration(task.Name) : null);
			WorldGenerator.CurrentGenerationProgress.End();
		}
		WorldGenerator.CurrentGenerationProgress = null;
		SystemLoader.OnWorldLoad();
	}

	private static void TryLoadWorldFile(string path, bool cloud, int tries)
	{
		SubworldSystem.LoadWorldFile(path, cloud);
		if (!WorldGen.loadFailed)
		{
			return;
		}
		switch (tries)
		{
		case 1:
			if (FileUtilities.Exists(path + ".bak", cloud))
			{
				WorldGen.worldBackup = true;
				FileUtilities.Copy(path, path + ".bad", cloud, true);
				FileUtilities.Copy(path + ".bak", path, cloud, true);
				FileUtilities.Delete(path + ".bak", cloud, false);
				string tMLPath = Path.ChangeExtension(path, ".twld");
				if (FileUtilities.Exists(tMLPath, cloud))
				{
					FileUtilities.Copy(tMLPath, tMLPath + ".bad", cloud, true);
				}
				if (FileUtilities.Exists(tMLPath + ".bak", cloud))
				{
					FileUtilities.Copy(tMLPath + ".bak", tMLPath, cloud, true);
					FileUtilities.Delete(tMLPath + ".bak", cloud, false);
				}
				break;
			}
			WorldGen.worldBackup = false;
			return;
		case 3:
		{
			FileUtilities.Copy(path, path + ".bak", cloud, true);
			FileUtilities.Copy(path + ".bad", path, cloud, true);
			FileUtilities.Delete(path + ".bad", cloud, false);
			string tMLPath2 = Path.ChangeExtension(path, ".twld");
			if (FileUtilities.Exists(tMLPath2, cloud))
			{
				FileUtilities.Copy(tMLPath2, tMLPath2 + ".bak", cloud, true);
			}
			if (FileUtilities.Exists(tMLPath2 + ".bad", cloud))
			{
				FileUtilities.Copy(tMLPath2 + ".bad", tMLPath2, cloud, true);
				FileUtilities.Delete(tMLPath2 + ".bad", cloud, false);
			}
			return;
		}
		}
		SubworldSystem.TryLoadWorldFile(path, cloud, tries++);
	}

	private static void LoadWorldFile(string path, bool cloud)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		bool flag = cloud && SocialAPI.Cloud != null;
		if (!FileUtilities.Exists(path, flag))
		{
			return;
		}
		if (SubworldSystem.current != null)
		{
			Main.ActiveWorldFileData = new WorldFileData(path, cloud);
		}
		try
		{
			BinaryReader reader = new BinaryReader((Stream)new MemoryStream(FileUtilities.ReadAllBytes(path, flag)));
			int status;
			try
			{
				status = ((SubworldSystem.current != null) ? SubworldSystem.current.ReadFile(reader) : WorldFile.LoadWorld_Version2(reader));
			}
			finally
			{
				((IDisposable)reader)?.Dispose();
			}
			if (Main.netMode == 2)
			{
				Console.Title = Main.worldName;
			}
			SystemLoader.OnWorldLoad();
			typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.IO.WorldIO")!.GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, new object[2] { path, flag });
			if (status != 0)
			{
				WorldGen.loadFailed = true;
				WorldGen.loadSuccess = false;
				return;
			}
			WorldGen.loadSuccess = true;
			WorldGen.loadFailed = false;
			if (SubworldSystem.current != null)
			{
				SubworldSystem.current.PostReadFile();
				SubworldSystem.cache?.ReadCopiedSubworldData();
				SubworldSystem.ReadCopiedMainWorldData();
			}
			else
			{
				SubworldSystem.PostLoadWorldFile();
				SubworldSystem.cache.ReadCopiedSubworldData();
				SubworldSystem.copiedData = null;
			}
		}
		catch
		{
			WorldGen.loadFailed = true;
			WorldGen.loadSuccess = false;
		}
	}

	internal static void PostLoadWorldFile()
	{
		GenVars.waterLine = Main.maxTilesY;
		Liquid.QuickWater(2, -1, -1);
		WorldGen.WaterCheck();
		Liquid.quickSettle = true;
		int updates = 0;
		int amount = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
		float num = 0f;
		while (Liquid.numLiquid > 0 && updates < 100000)
		{
			updates++;
			float progress = (float)(amount - Liquid.numLiquid + LiquidBuffer.numLiquidBuffer) / (float)amount;
			if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > amount)
			{
				amount = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
			}
			if (progress > num)
			{
				num = progress;
			}
			else
			{
				progress = num;
			}
			Main.set_statusText(Lang.gen[27].get_Value() + " " + (int)(progress * 100f / 2f + 50f) + "%");
			Liquid.UpdateLiquid();
		}
		Liquid.quickSettle = false;
		Main.weatherCounter = WorldGen.get_genRand().Next(3600, 18000);
		Cloud.resetClouds();
		WorldGen.WaterCheck();
		NPC.setFireFlyChance();
		if (Main.slimeRainTime > 0.0)
		{
			Main.StartSlimeRain(false);
		}
		NPC.SetWorldSpecificMonstersByWorldID();
	}
}
