using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using Terraria;
using Terraria.GameContent.NetModules;
using Terraria.Graphics.Light;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.Net.Sockets;
using Terraria.Utilities;

namespace SubworldLibrary;

public class SubworldLibrary : Mod
{
	private static event Manipulator AsyncSend
	{
		add
		{
			MonoModHooks.Modify((MethodBase)typeof(SocialSocket).GetMethod("Terraria.Net.Sockets.ISocket.AsyncSend", BindingFlags.Instance | BindingFlags.NonPublic), (Delegate)(object)value);
			MonoModHooks.Modify((MethodBase)typeof(TcpSocket).GetMethod("Terraria.Net.Sockets.ISocket.AsyncSend", BindingFlags.Instance | BindingFlags.NonPublic), (Delegate)(object)value);
		}
		remove
		{
			HookEndpointManager.Unmodify((MethodBase)typeof(SocialSocket).GetMethod("Terraria.Net.Sockets.ISocket.AsyncSend", BindingFlags.Instance | BindingFlags.NonPublic), (Delegate)(object)value);
			HookEndpointManager.Unmodify((MethodBase)typeof(TcpSocket).GetMethod("Terraria.Net.Sockets.ISocket.AsyncSend", BindingFlags.Instance | BindingFlags.NonPublic), (Delegate)(object)value);
		}
	}

	public override void Load()
	{
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected O, but got Unknown
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Expected O, but got Unknown
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Expected O, but got Unknown
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Expected O, but got Unknown
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Expected O, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Expected O, but got Unknown
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Expected O, but got Unknown
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Expected O, but got Unknown
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Expected O, but got Unknown
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Expected O, but got Unknown
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Expected O, but got Unknown
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Expected O, but got Unknown
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Expected O, but got Unknown
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Expected O, but got Unknown
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Expected O, but got Unknown
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Expected O, but got Unknown
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Expected O, but got Unknown
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Expected O, but got Unknown
		LocalizedText translation = Language.GetOrRegister("Mods.SubworldLibrary.Return");
		translation.AddTranslation(1, "Return");
		translation.AddTranslation(2, "Wiederkehren");
		translation.AddTranslation(3, "Ritorno");
		translation.AddTranslation(4, "Retour");
		translation.AddTranslation(5, "Regresar");
		translation.AddTranslation(6, "Возвращаться");
		translation.AddTranslation(7, "返回");
		translation.AddTranslation(8, "Regressar");
		translation.AddTranslation(9, "Wracać");
		LocalizationLoader.AddTranslation(translation)/* tModPorter Note: Removed. Use Language.GetOrRegister */;
		FieldInfo current = typeof(SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic);
		FieldInfo cache = typeof(SubworldSystem).GetField("cache", BindingFlags.Static | BindingFlags.NonPublic);
		FieldInfo hideUnderworld = typeof(SubworldSystem).GetField("hideUnderworld");
		MethodInfo normalUpdates = typeof(Subworld).GetMethod("get_NormalUpdates");
		MethodInfo shouldSave = typeof(Subworld).GetMethod("get_ShouldSave");
		if (Main.dedServ)
		{
			object obj = _003C_003Ec._003C_003E9__3_0;
			if (obj == null)
			{
				Manipulator val = delegate(ILContext il)
				{
					//IL_0045: Unknown result type (might be due to invalid IL or missing references)
					//IL_004b: Expected O, but got Unknown
					//IL_008a: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
					//IL_00db: Unknown result type (might be due to invalid IL or missing references)
					//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
					//IL_0108: Unknown result type (might be due to invalid IL or missing references)
					//IL_0114: Unknown result type (might be due to invalid IL or missing references)
					//IL_0120: Unknown result type (might be due to invalid IL or missing references)
					//IL_0140: Unknown result type (might be due to invalid IL or missing references)
					//IL_014c: Unknown result type (might be due to invalid IL or missing references)
					//IL_016c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0181: Unknown result type (might be due to invalid IL or missing references)
					//IL_018d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0199: Unknown result type (might be due to invalid IL or missing references)
					//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
					//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
					//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
					//IL_0225: Unknown result type (might be due to invalid IL or missing references)
					//IL_0233: Unknown result type (might be due to invalid IL or missing references)
					//IL_023f: Unknown result type (might be due to invalid IL or missing references)
					//IL_024c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0261: Unknown result type (might be due to invalid IL or missing references)
					//IL_0278: Unknown result type (might be due to invalid IL or missing references)
					//IL_0285: Unknown result type (might be due to invalid IL or missing references)
					//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
					//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
					//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
					//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
					//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
					//IL_0303: Unknown result type (might be due to invalid IL or missing references)
					//IL_030f: Unknown result type (might be due to invalid IL or missing references)
					//IL_031c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0347: Unknown result type (might be due to invalid IL or missing references)
					//IL_0367: Unknown result type (might be due to invalid IL or missing references)
					//IL_0375: Unknown result type (might be due to invalid IL or missing references)
					ConstructorInfo constructor = typeof(GameTime).GetConstructor(Type.EmptyTypes);
					MethodInfo method = typeof(Main).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);
					FieldInfo field = typeof(Main).GetField("saveTime", BindingFlags.Static | BindingFlags.NonPublic);
					ILCursor val54 = new ILCursor(il);
					if (val54.TryGotoNext((MoveType)2, new Func<Instruction, bool>[1]
					{
						(Instruction i) => ILPatternMatchingExt.MatchStindI1(i)
					}))
					{
						val54.Emit(OpCodes.Call, (MethodBase)typeof(SubworldSystem).GetMethod("LoadIntoSubworld", BindingFlags.Static | BindingFlags.NonPublic));
						ILLabel val55 = val54.DefineLabel();
						val54.Emit(OpCodes.Brfalse, (object)val55);
						val54.Emit(OpCodes.Ldarg_0);
						val54.Emit(OpCodes.Newobj, (MethodBase)constructor);
						val54.Emit(OpCodes.Callvirt, (MethodBase)method);
						val54.Emit(OpCodes.Newobj, (MethodBase)typeof(Stopwatch).GetConstructor(Type.EmptyTypes));
						val54.Emit(OpCodes.Stloc_1);
						val54.Emit(OpCodes.Ldloc_1);
						val54.Emit(OpCodes.Callvirt, (MethodBase)typeof(Stopwatch).GetMethod("Start"));
						val54.Emit(OpCodes.Ldc_I4_0);
						val54.Emit(OpCodes.Stsfld, typeof(Main).GetField("gameMenu"));
						val54.Emit(OpCodes.Ldc_R8, 16.666666666666668);
						val54.Emit(OpCodes.Stloc_2);
						val54.Emit(OpCodes.Ldloc_2);
						val54.Emit(OpCodes.Stloc_3);
						ILLabel val56 = val54.DefineLabel();
						val54.Emit(OpCodes.Br, (object)val56);
						ILLabel val57 = val54.DefineLabel();
						val54.MarkLabel(val57);
						val54.Emit(OpCodes.Call, (MethodBase)typeof(Main).Assembly.GetType("Terraria.ModLoader.Engine.ServerHangWatchdog")!.GetMethod("Checkin", BindingFlags.Static | BindingFlags.NonPublic));
						val54.Emit(OpCodes.Ldsfld, typeof(Netplay).GetField("HasClients"));
						ILLabel val58 = val54.DefineLabel();
						val54.Emit(OpCodes.Brfalse, (object)val58);
						val54.Emit(OpCodes.Ldarg_0);
						val54.Emit(OpCodes.Newobj, (MethodBase)constructor);
						val54.Emit(OpCodes.Callvirt, (MethodBase)method);
						ILLabel val59 = val54.DefineLabel();
						val54.Emit(OpCodes.Br, (object)val59);
						val54.MarkLabel(val58);
						val54.Emit(OpCodes.Ldsfld, field);
						val54.Emit(OpCodes.Callvirt, (MethodBase)typeof(Stopwatch).GetMethod("get_IsRunning"));
						val54.Emit(OpCodes.Brfalse, (object)val59);
						val54.Emit(OpCodes.Ldsfld, field);
						val54.Emit(OpCodes.Callvirt, (MethodBase)typeof(Stopwatch).GetMethod("Stop"));
						val54.Emit(OpCodes.Br, (object)val59);
						val54.MarkLabel(val59);
						val54.Emit(OpCodes.Ldloc_1);
						val54.Emit(OpCodes.Ldloc_2);
						val54.Emit(OpCodes.Ldloca, 3);
						val54.Emit(OpCodes.Call, (MethodBase)typeof(SubworldLibrary).GetMethod("Sleep", BindingFlags.Static | BindingFlags.NonPublic));
						val54.MarkLabel(val56);
						val54.Emit(OpCodes.Ldsfld, typeof(Netplay).GetField("Disconnect"));
						val54.Emit(OpCodes.Brfalse, (object)val57);
						val54.Emit(OpCodes.Ret);
						val54.MarkLabel(val55);
					}
				};
				obj = (object)val;
				_003C_003Ec._003C_003E9__3_0 = val;
			}
			Main.add_DedServ_PostModLoad((Manipulator)obj);
			if (!Program.LaunchParameters.ContainsKey("-subworld"))
			{
				object obj2 = _003C_003Ec._003C_003E9__3_1;
				if (obj2 == null)
				{
					Manipulator val2 = delegate(ILContext il)
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0008: Expected O, but got Unknown
						//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
						//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
						//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
						//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
						//IL_0106: Unknown result type (might be due to invalid IL or missing references)
						//IL_0113: Unknown result type (might be due to invalid IL or missing references)
						//IL_013a: Unknown result type (might be due to invalid IL or missing references)
						ILCursor val52 = new ILCursor(il);
						int num3 = default(int);
						int num4 = default(int);
						if (val52.TryGotoNext((MoveType)2, new Func<Instruction, bool>[1]
						{
							(Instruction i) => ILPatternMatchingExt.MatchCall(i, typeof(BitConverter), "ToUInt16")
						}) && ILPatternMatchingExt.MatchStloc(val52.get_Instrs().get_Item(val52.get_Index()), ref num3) && val52.TryGotoNext((MoveType)2, new Func<Instruction, bool>[2]
						{
							(Instruction i) => ILPatternMatchingExt.MatchCallvirt(i, typeof(Stream), "get_Position"),
							(Instruction i) => ILPatternMatchingExt.MatchStloc(i, ref num4)
						}))
						{
							val52.Emit(OpCodes.Ldsfld, typeof(NetMessage).GetField("buffer"));
							val52.Emit(OpCodes.Ldarg_0);
							val52.Emit(OpCodes.Ldelem_Ref);
							val52.Emit(OpCodes.Ldloc_2);
							val52.Emit(OpCodes.Ldloc, num3);
							val52.Emit(OpCodes.Call, (MethodBase)typeof(SubworldLibrary).GetMethod("DenyRead"));
							ILLabel val53 = val52.DefineLabel();
							val52.Emit(OpCodes.Brtrue, (object)val53);
							if (val52.TryGotoNext(new Func<Instruction, bool>[4]
							{
								(Instruction i) => ILPatternMatchingExt.MatchLdsfld(i, typeof(NetMessage), "buffer"),
								(Instruction i) => ILPatternMatchingExt.MatchLdarg(i, 0),
								(Instruction i) => ILPatternMatchingExt.MatchLdelemRef(i),
								(Instruction i) => ILPatternMatchingExt.MatchLdfld(i, typeof(MessageBuffer), "reader")
							}))
							{
								val52.MarkLabel(val53);
							}
						}
					};
					obj2 = (object)val2;
					_003C_003Ec._003C_003E9__3_1 = val2;
				}
				NetMessage.add_CheckBytes((Manipulator)obj2);
				object obj3 = _003C_003Ec._003C_003E9__3_2;
				if (obj3 == null)
				{
					Manipulator val3 = delegate(ILContext il)
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0008: Expected O, but got Unknown
						//IL_0009: Unknown result type (might be due to invalid IL or missing references)
						//IL_0015: Unknown result type (might be due to invalid IL or missing references)
						//IL_0021: Unknown result type (might be due to invalid IL or missing references)
						//IL_002d: Unknown result type (might be due to invalid IL or missing references)
						//IL_0039: Unknown result type (might be due to invalid IL or missing references)
						//IL_0046: Unknown result type (might be due to invalid IL or missing references)
						//IL_006d: Unknown result type (might be due to invalid IL or missing references)
						//IL_007a: Unknown result type (might be due to invalid IL or missing references)
						ILCursor val50 = new ILCursor(il);
						val50.Emit(OpCodes.Ldarg_0);
						val50.Emit(OpCodes.Ldarg_1);
						val50.Emit(OpCodes.Ldarg_2);
						val50.Emit(OpCodes.Ldarg_3);
						val50.Emit(OpCodes.Ldarga, 5);
						val50.Emit(OpCodes.Call, (MethodBase)typeof(SubworldLibrary).GetMethod("DenySend"));
						ILLabel val51 = val50.DefineLabel();
						val50.Emit(OpCodes.Brfalse, (object)val51);
						val50.Emit(OpCodes.Ret);
						val50.MarkLabel(val51);
					};
					obj3 = (object)val3;
					_003C_003Ec._003C_003E9__3_2 = val3;
				}
				SubworldLibrary.AsyncSend += (Manipulator)obj3;
			}
		}
		else
		{
			Main.add_DoDraw((Manipulator)delegate(ILContext il)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_013f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0161: Unknown result type (might be due to invalid IL or missing references)
				//IL_0173: Unknown result type (might be due to invalid IL or missing references)
				//IL_017f: Unknown result type (might be due to invalid IL or missing references)
				//IL_019f: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_021d: Unknown result type (might be due to invalid IL or missing references)
				//IL_023f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0272: Unknown result type (might be due to invalid IL or missing references)
				//IL_0294: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
				ILCursor val47 = new ILCursor(il);
				if (val47.TryGotoNext((MoveType)2, new Func<Instruction, bool>[1]
				{
					(Instruction i) => ILPatternMatchingExt.MatchStsfld(i, typeof(Main), "HoverItem")
				}))
				{
					val47.Emit(OpCodes.Ldsfld, typeof(Main).GetField("gameMenu"));
					ILLabel val48 = val47.DefineLabel();
					val47.Emit(OpCodes.Brfalse, (object)val48);
					val47.Emit(OpCodes.Ldsfld, current);
					ILLabel val49 = val47.DefineLabel();
					val47.Emit(OpCodes.Brfalse, (object)val49);
					val47.Emit(OpCodes.Ldc_R4, 1f);
					val47.Emit(OpCodes.Dup);
					val47.Emit(OpCodes.Dup);
					val47.Emit(OpCodes.Stsfld, typeof(Main).GetField("_uiScaleWanted", BindingFlags.Static | BindingFlags.NonPublic));
					val47.Emit(OpCodes.Stsfld, typeof(Main).GetField("_uiScaleUsed", BindingFlags.Static | BindingFlags.NonPublic));
					val47.Emit(OpCodes.Call, (MethodBase)typeof(Matrix).GetMethod("CreateScale", new Type[1] { typeof(float) }));
					val47.Emit(OpCodes.Stsfld, typeof(Main).GetField("_uiScaleMatrix", BindingFlags.Static | BindingFlags.NonPublic));
					val47.Emit(OpCodes.Ldsfld, current);
					val47.Emit(OpCodes.Ldarg_0);
					val47.Emit(OpCodes.Callvirt, (MethodBase)typeof(Subworld).GetMethod("DrawSetup"));
					val47.Emit(OpCodes.Ret);
					val47.MarkLabel(val49);
					val47.Emit(OpCodes.Ldsfld, cache);
					val47.Emit(OpCodes.Brfalse, (object)val48);
					val47.Emit(OpCodes.Ldc_R4, 1f);
					val47.Emit(OpCodes.Dup);
					val47.Emit(OpCodes.Dup);
					val47.Emit(OpCodes.Stsfld, typeof(Main).GetField("_uiScaleWanted", BindingFlags.Static | BindingFlags.NonPublic));
					val47.Emit(OpCodes.Stsfld, typeof(Main).GetField("_uiScaleUsed", BindingFlags.Static | BindingFlags.NonPublic));
					val47.Emit(OpCodes.Call, (MethodBase)typeof(Matrix).GetMethod("CreateScale", new Type[1] { typeof(float) }));
					val47.Emit(OpCodes.Stsfld, typeof(Main).GetField("_uiScaleMatrix", BindingFlags.Static | BindingFlags.NonPublic));
					val47.Emit(OpCodes.Ldsfld, cache);
					val47.Emit(OpCodes.Ldarg_0);
					val47.Emit(OpCodes.Callvirt, (MethodBase)typeof(Subworld).GetMethod("DrawSetup"));
					val47.Emit(OpCodes.Ret);
					val47.MarkLabel(val48);
				}
			});
			Main.add_DrawBackground((Manipulator)delegate(ILContext il)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				ILCursor val44 = new ILCursor(il);
				if (val44.TryGotoNext(new Func<Instruction, bool>[1]
				{
					(Instruction i) => ILPatternMatchingExt.MatchLdcI4(i, 330)
				}))
				{
					val44.Emit(OpCodes.Ldsfld, hideUnderworld);
					ILLabel val45 = val44.DefineLabel();
					val44.Emit(OpCodes.Brfalse, (object)val45);
					val44.Emit(OpCodes.Conv_R8);
					ILLabel val46 = val44.DefineLabel();
					val44.Emit(OpCodes.Br, (object)val46);
					val44.MarkLabel(val45);
					if (val44.TryGotoNext(new Func<Instruction, bool>[2]
					{
						(Instruction i) => ILPatternMatchingExt.MatchStloc(i, 2),
						(Instruction i) => ILPatternMatchingExt.MatchLdcR4(i, 255f)
					}))
					{
						val44.MarkLabel(val46);
					}
				}
			});
			Main.add_OldDrawBackground((Manipulator)delegate(ILContext il)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				ILCursor val41 = new ILCursor(il);
				if (val41.TryGotoNext(new Func<Instruction, bool>[1]
				{
					(Instruction i) => ILPatternMatchingExt.MatchLdcI4(i, 230)
				}))
				{
					val41.Emit(OpCodes.Ldsfld, hideUnderworld);
					ILLabel val42 = val41.DefineLabel();
					val41.Emit(OpCodes.Brfalse, (object)val42);
					val41.Emit(OpCodes.Conv_R8);
					ILLabel val43 = val41.DefineLabel();
					val41.Emit(OpCodes.Br, (object)val43);
					val41.MarkLabel(val42);
					if (val41.TryGotoNext(new Func<Instruction, bool>[2]
					{
						(Instruction i) => ILPatternMatchingExt.MatchStloc(i, 18),
						(Instruction i) => ILPatternMatchingExt.MatchLdcI4(i, 0)
					}))
					{
						val41.MarkLabel(val43);
					}
				}
			});
			IngameOptions.add_Draw((Manipulator)delegate(ILContext il)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0191: Unknown result type (might be due to invalid IL or missing references)
				//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01de: Unknown result type (might be due to invalid IL or missing references)
				//IL_02da: Unknown result type (might be due to invalid IL or missing references)
				//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
				ILCursor val38 = new ILCursor(il);
				if (val38.TryGotoNext(new Func<Instruction, bool>[2]
				{
					(Instruction i) => ILPatternMatchingExt.MatchLdsfld(i, typeof(Lang), "inter"),
					(Instruction i) => ILPatternMatchingExt.MatchLdcI4(i, 35)
				}))
				{
					val38.Emit(OpCodes.Ldsfld, current);
					ILLabel val39 = val38.DefineLabel();
					val38.Emit(OpCodes.Brfalse, (object)val39);
					val38.Emit(OpCodes.Ldstr, "Mods.SubworldLibrary.Return");
					val38.Emit(OpCodes.Call, (MethodBase)typeof(Language).GetMethod("GetTextValue", new Type[1] { typeof(string) }));
					ILLabel val40 = val38.DefineLabel();
					val38.Emit(OpCodes.Br, (object)val40);
					val38.MarkLabel(val39);
					if (val38.TryGotoNext((MoveType)2, new Func<Instruction, bool>[1]
					{
						(Instruction i) => ILPatternMatchingExt.MatchCallvirt(i, typeof(LocalizedText), "get_Value")
					}))
					{
						val38.MarkLabel(val40);
						if (val38.TryGotoNext(new Func<Instruction, bool>[2]
						{
							(Instruction i) => ILPatternMatchingExt.MatchLdnull(i),
							(Instruction i) => ILPatternMatchingExt.MatchCall(i, typeof(WorldGen), "SaveAndQuit")
						}))
						{
							val38.Emit(OpCodes.Ldsfld, current);
							val39 = val38.DefineLabel();
							val38.Emit(OpCodes.Brfalse, (object)val39);
							val38.Emit(OpCodes.Call, (MethodBase)typeof(SubworldSystem).GetMethod("Exit"));
							val40 = val38.DefineLabel();
							val38.Emit(OpCodes.Br, (object)val40);
							val38.MarkLabel(val39);
							if (val38.TryGotoNext((MoveType)2, new Func<Instruction, bool>[1]
							{
								(Instruction i) => ILPatternMatchingExt.MatchCall(i, typeof(WorldGen), "SaveAndQuit")
							}))
							{
								val38.MarkLabel(val40);
								int num2 = default(int);
								int num = default(int);
								if (val38.TryGotoPrev((MoveType)1, new Func<Instruction, bool>[4]
								{
									(Instruction i) => ILPatternMatchingExt.MatchLdloc(i, ref num2),
									(Instruction i) => ILPatternMatchingExt.MatchLdcI4(i, 1),
									(Instruction i) => ILPatternMatchingExt.MatchAdd(i),
									(Instruction i) => ILPatternMatchingExt.MatchStloc(i, ref num)
								}))
								{
									val38.Emit(OpCodes.Ldsfld, typeof(SubworldSystem).GetField("noReturn"));
									val38.Emit(OpCodes.Brtrue, (object)val40);
								}
							}
						}
					}
				}
			});
			TileLightScanner.add_GetTileLight((Manipulator)delegate(ILContext il)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0156: Unknown result type (might be due to invalid IL or missing references)
				//IL_016f: Unknown result type (might be due to invalid IL or missing references)
				ILCursor val36 = new ILCursor(il);
				if (val36.TryGotoNext((MoveType)2, new Func<Instruction, bool>[1]
				{
					(Instruction i) => ILPatternMatchingExt.MatchStloc(i, 1)
				}))
				{
					val36.Emit(OpCodes.Ldsfld, current);
					ILLabel val37 = val36.DefineLabel();
					val36.Emit(OpCodes.Brfalse, (object)val37);
					val36.Emit(OpCodes.Ldsfld, current);
					val36.Emit(OpCodes.Ldloc_0);
					val36.Emit(OpCodes.Ldarg_1);
					val36.Emit(OpCodes.Ldarg_2);
					val36.Emit(OpCodes.Ldloca, 1);
					val36.Emit(OpCodes.Ldarg_3);
					val36.Emit(OpCodes.Callvirt, (MethodBase)typeof(Subworld).GetMethod("GetLight"));
					val36.Emit(OpCodes.Brfalse, (object)val37);
					val36.Emit(OpCodes.Ret);
					val36.MarkLabel(val37);
					if (val36.TryGotoNext((MoveType)1, new Func<Instruction, bool>[2]
					{
						(Instruction i) => ILPatternMatchingExt.MatchLdarg(i, 2),
						(Instruction i) => ILPatternMatchingExt.MatchCall(i, typeof(Main), "get_UnderworldLayer")
					}))
					{
						val36.Emit(OpCodes.Ldsfld, hideUnderworld);
						val37 = val36.DefineLabel();
						val36.Emit(OpCodes.Brtrue, (object)val37);
						if (val36.TryGotoNext((MoveType)2, new Func<Instruction, bool>[1]
						{
							(Instruction i) => ILPatternMatchingExt.MatchCall(i, typeof(TileLightScanner), "ApplyHellLight")
						}))
						{
							val36.MarkLabel(val37);
						}
					}
				}
			});
			Player.add_UpdateBiomes((Manipulator)delegate(ILContext il)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				ILCursor val33 = new ILCursor(il);
				if (val33.TryGotoNext((MoveType)2, new Func<Instruction, bool>[1]
				{
					(Instruction i) => ILPatternMatchingExt.MatchStloc(i, 9)
				}))
				{
					val33.Emit(OpCodes.Ldsfld, hideUnderworld);
					ILLabel val34 = val33.DefineLabel();
					val33.Emit(OpCodes.Brfalse, (object)val34);
					val33.Emit(OpCodes.Ldc_I4_0);
					ILLabel val35 = val33.DefineLabel();
					val33.Emit(OpCodes.Br, (object)val35);
					val33.MarkLabel(val34);
					if (val33.TryGotoNext(new Func<Instruction, bool>[1]
					{
						(Instruction i) => ILPatternMatchingExt.MatchStloc(i, 10)
					}))
					{
						val33.MarkLabel(val35);
					}
				}
			});
			Main.add_DrawUnderworldBackground((Manipulator)delegate(ILContext il)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				ILCursor val31 = new ILCursor(il);
				val31.Emit(OpCodes.Ldsfld, hideUnderworld);
				ILLabel val32 = val31.DefineLabel();
				val31.Emit(OpCodes.Brfalse, (object)val32);
				val31.Emit(OpCodes.Ret);
				val31.MarkLabel(val32);
			});
			Netplay.add_AddCurrentServerToRecentList((Manipulator)delegate(ILContext il)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				ILCursor val29 = new ILCursor(il);
				val29.Emit(OpCodes.Ldsfld, current);
				ILLabel val30 = val29.DefineLabel();
				val29.Emit(OpCodes.Brfalse, (object)val30);
				val29.Emit(OpCodes.Ret);
				val29.MarkLabel(val30);
			});
		}
		object obj4 = _003C_003Ec._003C_003E9__3_11;
		if (obj4 == null)
		{
			Manipulator val4 = delegate(ILContext il)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				ILCursor val28 = new ILCursor(il);
				if (val28.TryGotoNext((MoveType)2, new Func<Instruction, bool>[1]
				{
					(Instruction i) => ILPatternMatchingExt.MatchCall(i, typeof(WorldFile), "saveWorld")
				}))
				{
					val28.Emit(OpCodes.Call, (MethodBase)typeof(SubworldSystem).GetMethod("GenerateSubworlds", BindingFlags.Static | BindingFlags.NonPublic));
				}
			};
			obj4 = (object)val4;
			_003C_003Ec._003C_003E9__3_11 = val4;
		}
		WorldGen.add_do_worldGenCallBack((Manipulator)obj4);
		object obj5 = _003C_003Ec._003C_003E9__3_12;
		if (obj5 == null)
		{
			Manipulator val5 = delegate(ILContext il)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				ILCursor val27 = new ILCursor(il);
				val27.Emit(OpCodes.Ldarg_0);
				val27.Emit(OpCodes.Call, (MethodBase)typeof(SubworldSystem).GetMethod("EraseSubworlds", BindingFlags.Static | BindingFlags.NonPublic));
			};
			obj5 = (object)val5;
			_003C_003Ec._003C_003E9__3_12 = val5;
		}
		Main.add_EraseWorld((Manipulator)obj5);
		Main.add_DoUpdateInWorld((Manipulator)delegate(ILContext il)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			ILCursor val24 = new ILCursor(il);
			if (val24.TryGotoNext((MoveType)2, new Func<Instruction, bool>[1]
			{
				(Instruction i) => ILPatternMatchingExt.MatchCall(i, typeof(SystemLoader), "PreUpdateTime")
			}))
			{
				val24.Emit(OpCodes.Ldsfld, current);
				ILLabel val25 = val24.DefineLabel();
				val24.Emit(OpCodes.Brfalse, (object)val25);
				val24.Emit(OpCodes.Ldsfld, current);
				val24.Emit(OpCodes.Callvirt, (MethodBase)normalUpdates);
				ILLabel val26 = val24.DefineLabel();
				val24.Emit(OpCodes.Brfalse, (object)val26);
				val24.MarkLabel(val25);
				if (val24.TryGotoNext(new Func<Instruction, bool>[1]
				{
					(Instruction i) => ILPatternMatchingExt.MatchCall(i, typeof(SystemLoader), "PostUpdateTime")
				}))
				{
					val24.MarkLabel(val26);
				}
			}
		});
		WorldGen.add_UpdateWorld((Manipulator)delegate(ILContext il)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			ILCursor val21 = new ILCursor(il);
			if (val21.TryGotoNext(new Func<Instruction, bool>[1]
			{
				(Instruction i) => ILPatternMatchingExt.MatchCall(i, typeof(WorldGen), "UpdateWorld_Inner")
			}))
			{
				val21.Emit(OpCodes.Ldsfld, current);
				ILLabel val22 = val21.DefineLabel();
				val21.Emit(OpCodes.Brfalse, (object)val22);
				val21.Emit(OpCodes.Ldsfld, current);
				val21.Emit(OpCodes.Callvirt, (MethodBase)normalUpdates);
				ILLabel val23 = val21.DefineLabel();
				val21.Emit(OpCodes.Brfalse, (object)val23);
				val21.MarkLabel(val22);
				val21.set_Index(val21.get_Index() + 1);
				val21.MarkLabel(val23);
			}
		});
		Player.add_Update((Manipulator)delegate(ILContext il)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			ILCursor val17 = new ILCursor(il);
			FieldReference val20 = default(FieldReference);
			if (val17.TryGotoNext((MoveType)1, new Func<Instruction, bool>[2]
			{
				(Instruction i) => ILPatternMatchingExt.MatchLdsfld(i, ref val20),
				(Instruction i) => ILPatternMatchingExt.MatchLdcI4(i, 4200)
			}))
			{
				val17.Emit(OpCodes.Ldsfld, current);
				ILLabel val18 = val17.DefineLabel();
				val17.Emit(OpCodes.Brfalse, (object)val18);
				val17.Emit(OpCodes.Ldsfld, current);
				val17.Emit(OpCodes.Callvirt, (MethodBase)normalUpdates);
				ILLabel val19 = val17.DefineLabel();
				val17.Emit(OpCodes.Brfalse, (object)val19);
				val17.MarkLabel(val18);
				if (val17.TryGotoNext((MoveType)2, new Func<Instruction, bool>[3]
				{
					(Instruction i) => ILPatternMatchingExt.MatchLdloc(i, 3),
					(Instruction i) => ILPatternMatchingExt.MatchMul(i),
					(Instruction i) => ILPatternMatchingExt.MatchStfld(i, typeof(Player), "gravity")
				}))
				{
					val17.MarkLabel(val19);
				}
			}
		});
		NPC.add_UpdateNPC_UpdateGravity((Manipulator)delegate(ILContext il)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			ILCursor val13 = new ILCursor(il);
			FieldReference val16 = default(FieldReference);
			if (val13.TryGotoNext((MoveType)1, new Func<Instruction, bool>[2]
			{
				(Instruction i) => ILPatternMatchingExt.MatchLdsfld(i, ref val16),
				(Instruction i) => ILPatternMatchingExt.MatchLdcI4(i, 4200)
			}))
			{
				val13.Emit(OpCodes.Ldsfld, current);
				ILLabel val14 = val13.DefineLabel();
				val13.Emit(OpCodes.Brfalse, (object)val14);
				val13.Emit(OpCodes.Ldsfld, current);
				val13.Emit(OpCodes.Callvirt, (MethodBase)normalUpdates);
				ILLabel val15 = val13.DefineLabel();
				val13.Emit(OpCodes.Brfalse, (object)val15);
				val13.MarkLabel(val14);
				if (val13.TryGotoNext((MoveType)2, new Func<Instruction, bool>[3]
				{
					(Instruction i) => ILPatternMatchingExt.MatchLdloc(i, 1),
					(Instruction i) => ILPatternMatchingExt.MatchMul(i),
					(Instruction i) => ILPatternMatchingExt.MatchStsfld(i, typeof(NPC), "gravity")
				}))
				{
					val13.MarkLabel(val15);
				}
			}
		});
		Liquid.add_Update((Manipulator)delegate(ILContext il)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Expected O, but got Unknown
			ILCursor val11 = new ILCursor(il);
			if (val11.TryGotoNext(new Func<Instruction, bool>[3]
			{
				(Instruction i) => ILPatternMatchingExt.MatchLdarg(i, 0),
				(Instruction i) => ILPatternMatchingExt.MatchLdfld(i, typeof(Liquid), "y"),
				(Instruction i) => ILPatternMatchingExt.MatchCall(i, typeof(Main), "get_UnderworldLayer")
			}))
			{
				val11.Emit(OpCodes.Ldsfld, current);
				ILLabel val12 = val11.DefineLabel();
				val11.Emit(OpCodes.Brfalse, (object)val12);
				val11.Emit(OpCodes.Ldsfld, current);
				val11.Emit(OpCodes.Callvirt, (MethodBase)normalUpdates);
				val11.Emit(OpCodes.Brfalse, (object)(ILLabel)val11.get_Instrs().get_Item(val11.get_Index() + 3).get_Operand());
				val11.MarkLabel(val12);
			}
		});
		Player.add_SavePlayer((Manipulator)delegate(ILContext il)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			ILCursor val8 = new ILCursor(il);
			if (val8.TryGotoNext(new Func<Instruction, bool>[1]
			{
				(Instruction i) => ILPatternMatchingExt.MatchCall(i, typeof(Player), "InternalSaveMap")
			}))
			{
				val8.set_Index(val8.get_Index() - 3);
				val8.Emit(OpCodes.Ldsfld, cache);
				ILLabel val9 = val8.DefineLabel();
				val8.Emit(OpCodes.Brfalse, (object)val9);
				val8.Emit(OpCodes.Ldsfld, cache);
				val8.Emit(OpCodes.Callvirt, (MethodBase)shouldSave);
				ILLabel val10 = val8.DefineLabel();
				val8.Emit(OpCodes.Brfalse, (object)val10);
				val8.MarkLabel(val9);
				if (val8.TryGotoNext((MoveType)1, new Func<Instruction, bool>[1]
				{
					(Instruction i) => ILPatternMatchingExt.MatchLdsfld(i, typeof(Main), "ServerSideCharacter")
				}))
				{
					val8.MarkLabel(val10);
					val8.Emit(OpCodes.Ldsfld, cache);
					val9 = val8.DefineLabel();
					val8.Emit(OpCodes.Brfalse, (object)val9);
					val8.Emit(OpCodes.Ldsfld, cache);
					val8.Emit(OpCodes.Callvirt, (MethodBase)typeof(Subworld).GetMethod("get_NoPlayerSaving"));
					val10 = val8.DefineLabel();
					val8.Emit(OpCodes.Brtrue, (object)val10);
					val8.MarkLabel(val9);
					if (val8.TryGotoNext((MoveType)2, new Func<Instruction, bool>[1]
					{
						(Instruction i) => ILPatternMatchingExt.MatchCall(i, typeof(FileUtilities), "ProtectedInvoke")
					}))
					{
						val8.MarkLabel(val10);
					}
				}
			}
		});
		WorldFile.add_SaveWorld_bool_bool((Manipulator)delegate(ILContext il)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			ILCursor val6 = new ILCursor(il);
			val6.Emit(OpCodes.Ldsfld, cache);
			ILLabel val7 = val6.DefineLabel();
			val6.Emit(OpCodes.Brfalse, (object)val7);
			val6.Emit(OpCodes.Ldsfld, cache);
			val6.Emit(OpCodes.Callvirt, (MethodBase)shouldSave);
			val6.Emit(OpCodes.Brtrue, (object)val7);
			val6.Emit(OpCodes.Ret);
			val6.MarkLabel(val7);
		});
	}

	public static bool DenyRead(MessageBuffer buffer, int start, int length)
	{
		if (buffer.readBuffer[start + 2] == 250 && ((ModNet.get_NetModCount() < 256) ? buffer.readBuffer[start + 3] : BitConverter.ToUInt16(buffer.readBuffer, start + 3)) == ((Mod)ModContent.GetInstance<SubworldLibrary>()).get_NetID())
		{
			return false;
		}
		if (!SubworldSystem.playerLocations.TryGetValue(Netplay.Clients[buffer.whoAmI].Socket.GetRemoteAddress(), out var id))
		{
			return false;
		}
		Netplay.Clients[buffer.whoAmI].TimeOutTimer = 0;
		byte[] packet = new byte[length + 1];
		packet[0] = (byte)buffer.whoAmI;
		Buffer.BlockCopy(buffer.readBuffer, start, packet, 1, length);
		SubworldSystem.SendToPipe(SubworldSystem.pipes[id], packet);
		return packet[3] != 2 && (packet[3] != 82 || BitConverter.ToUInt16(packet, 4) != NetManager.Instance.GetId<NetBestiaryModule>());
	}

	public static bool DenySend(ISocket socket, byte[] data, int start, int length, ref object state)
	{
		if (Thread.CurrentThread.Name == "Subserver Packet Thread")
		{
			return false;
		}
		return SubworldSystem.playerLocations.ContainsKey(socket.GetRemoteAddress());
	}

	private static void Sleep(Stopwatch stopwatch, double delta, ref double target)
	{
		double now = stopwatch.get_ElapsedMilliseconds();
		double remaining = target - now;
		target += delta;
		if (target < now)
		{
			target = now + delta;
		}
		if (remaining <= 0.0)
		{
			Thread.Sleep(0);
		}
		else
		{
			Thread.Sleep((int)remaining);
		}
	}

	public override void HandlePacket(BinaryReader reader, int whoAmI)
	{
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Expected O, but got Unknown
		switch (reader.ReadByte())
		{
		case 0:
			if (Main.netMode == 2)
			{
				ushort id = reader.ReadUInt16();
				RemoteAddress address = Netplay.Clients[whoAmI].Socket.GetRemoteAddress();
				if (address != null)
				{
					ModPacket packet = ((Mod)this).GetPacket(256);
					((BinaryWriter)packet).Write((byte)0);
					((BinaryWriter)packet).Write(id);
					packet.Send(whoAmI, -1);
					((Entity)Main.player[whoAmI]).active = false;
					SubworldSystem.StartSubserver(id);
					SubworldSystem.playerLocations[address] = id;
				}
			}
			else
			{
				Task.Factory.StartNew(SubworldSystem.ExitWorldCallBack, SubworldSystem.subworlds[reader.ReadUInt16()]);
			}
			break;
		case 1:
			if (Main.netMode == 2)
			{
				RemoteAddress address2 = Netplay.Clients[whoAmI].Socket.GetRemoteAddress();
				if (address2 != null)
				{
					byte[] obj = new byte[7] { 0, 6, 0, 250, 0, 2, 0 };
					obj[0] = (byte)whoAmI;
					obj[4] = (byte)((Mod)this).get_NetID();
					obj[6] = (byte)whoAmI;
					SubworldSystem.SendToPipe(SubworldSystem.pipes[SubworldSystem.playerLocations[address2]], obj);
					SubworldSystem.playerLocations.Remove(address2);
					Netplay.Clients[whoAmI].State = 0;
					ModPacket packet2 = ((Mod)this).GetPacket(256);
					((BinaryWriter)packet2).Write((byte)1);
					packet2.Send(whoAmI, -1);
				}
			}
			else
			{
				Task.Factory.StartNew(SubworldSystem.ExitWorldCallBack, null);
			}
			break;
		case 2:
		{
			byte index = reader.ReadByte();
			Main.player[index] = new Player();
			RemoteClient client = Netplay.Clients[index];
			client.IsActive = false;
			client.Socket = null;
			client.State = 0;
			client.ResetSections();
			client.SpamClear();
			NetMessage.SyncDisconnectedPlayer((int)index);
			bool connection = false;
			for (int i = 0; i < 255; i++)
			{
				if (Netplay.Clients[i].State > 0)
				{
					connection = true;
					break;
				}
			}
			if (!connection)
			{
				Netplay.Disconnect = true;
				Netplay.HasClients = false;
			}
			break;
		}
		}
	}
}
