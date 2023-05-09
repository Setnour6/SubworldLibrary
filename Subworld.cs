using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace SubworldLibrary;

public abstract class Subworld : ModType
{
	public abstract int Width { get; }

	public abstract int Height { get; }

	public abstract List<GenPass> Tasks { get; }

	public virtual WorldGenConfiguration Config => null;

	public virtual bool ShouldSave => false;

	public virtual bool NoPlayerSaving => false;

	public virtual bool NormalUpdates => false;

	protected sealed override void Register()
	{
		ModTypeLookup<Subworld>.Register(this);
		SubworldSystem.subworlds.Add(this);
	}

	public sealed override void SetupContent()
	{
		((ModType)this).SetStaticDefaults();
	}

	public virtual void OnEnter()
	{
	}

	public virtual void OnExit()
	{
	}

	public virtual void CopyMainWorldData()
	{
	}

	public virtual void CopySubworldData()
	{
	}

	public virtual void ReadCopiedMainWorldData()
	{
	}

	public virtual void ReadCopiedSubworldData()
	{
	}

	public virtual void OnLoad()
	{
	}

	public virtual void OnUnload()
	{
	}

	public virtual int ReadFile(BinaryReader reader)
	{
		int status = WorldFile.LoadWorld_Version2(reader);
		((FileData)Main.ActiveWorldFileData).Name = Main.worldName;
		((FileData)Main.ActiveWorldFileData).Metadata = Main.WorldFileMetadata;
		return status;
	}

	public virtual void PostReadFile()
	{
		SubworldSystem.PostLoadWorldFile();
	}

	public virtual void DrawSetup(GameTime gameTime)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		PlayerInput.SetZoom_Unscaled();
		((Game)Main.instance).get_GraphicsDevice().Clear(Color.get_Black());
		Main.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, (Effect)null, Main.get_UIScaleMatrix());
		this.DrawMenu(gameTime);
		Main.DrawCursor(Main.DrawThickCursor(false), false);
		Main.spriteBatch.End();
	}

	public virtual void DrawMenu(GameTime gameTime)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.get_Value(), Main.get_statusText(), new Vector2((float)Main.screenWidth, (float)Main.screenHeight) / 2f - FontAssets.DeathText.get_Value().MeasureString(Main.get_statusText()) / 2f, Color.get_White());
	}

	public virtual bool GetLight(Tile tile, int x, int y, ref FastRandom rand, ref Vector3 color)
	{
		return false;
	}
}
