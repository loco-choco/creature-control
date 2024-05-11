using UnityEngine;

using OWML.Common;
using OWML.ModHelper;

namespace CreatureControl;

public class CreatureControl : ModBehaviour
{
  public static IGizmosAPI GizmosAPI;
	private void Awake()
	{
		// You won't be able to access OWML's mod helper in Awake.
		// So you probably don't want to do anything here.
		// Use Start() instead.
	}

	private void Start()
	{
		// Starting here, you'll have access to OWML's mod helper.
		ModHelper.Console.WriteLine($"My mod {nameof(CreatureControl)} is loaded!", MessageType.Success);
    GizmosAPI = ModHelper.Interaction.TryGetModApi<IGizmosAPI>("Locochoco.GizmosLibrary");
		// Example of accessing game code.
		LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
		{
			if (loadScene != OWScene.SolarSystem) return;
			ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
      var graph_manipulator = GameObject.CreatePrimitive(PrimitiveType.Cube);
      graph_manipulator.layer = 21; //OWLayerMask.interactMask;
      graph_manipulator.AddComponent<GraphManipulatorItem>().GizmosAPI = GizmosAPI;
      graph_manipulator.GetComponent<GraphManipulatorItem>().ModConsole = ModHelper.Console;
      graph_manipulator.transform.parent = Locator.GetAstroObject(AstroObject.Name.TimberHearth).transform;
      graph_manipulator.transform.position = FindObjectOfType<PlayerBody>().transform.position;

      var path_gen_manipulator = GameObject.CreatePrimitive(PrimitiveType.Cube);
      path_gen_manipulator.layer = 21; //OWLayerMask.interactMask;
      path_gen_manipulator.AddComponent<Vector3AStar>();
      path_gen_manipulator.AddComponent<PathGeneratorManipulatorItem>().GizmosAPI = GizmosAPI;
      //path_gen_manipulator.GetComponent<PathGeneratorManipulatorItem>().ModConsole = ModHelper.Console;
      PathGeneratorManipulatorItem.ModConsole = ModHelper.Console;
      path_gen_manipulator.transform.parent = Locator.GetAstroObject(AstroObject.Name.TimberHearth).transform;
      path_gen_manipulator.transform.position = FindObjectOfType<PlayerBody>().transform.position;
		};
	}
}

