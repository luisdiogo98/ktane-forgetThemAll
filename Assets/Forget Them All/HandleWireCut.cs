using UnityEngine;

public class HandleWireCut : MonoBehaviour {

	public KMSelectable wireSelectable;
	public MeshRenderer wireUncutMesh, wireCutMesh;
	public GameObject wireUncut, wireCut, wireHL;

	// Use this for initialization
	void Start () {
		wireUncut.SetActive(true);
		wireHL.SetActive(true);
		wireCut.SetActive(false);
		wireSelectable.OnInteract += delegate ()
		{
			// Set the wire to indicate that its cut already
			wireSelectable.enabled = false;
			wireUncut.SetActive(false);
			wireHL.SetActive(false);
			wireCut.SetActive(true);
			return false;
		};

	}

	public void UpdateWireColor(Material color) // Set the wire's material
	{
		wireUncutMesh.material = color;
		wireCutMesh.material = color;
	}

	// Update is called once per frame
	void Update () {

	}
}
