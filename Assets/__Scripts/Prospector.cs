using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Prospector : MonoBehaviour {

	static public Prospector 	S;

	[Header("Set in Inspector")]
	public TextAsset deckXML;
    public TextAsset layoutXML;

    public float xOffset = 3;
    public float yOffset = -2.5f;

    public Vector3 layoutCenter;


	[Header("Set Dynamically")]
	public Deck	deck;
    public Layout layout;
    public List<CardProspector> drawPile;

    public Transform layoutAnchor;
    public CardProspector target;

    public List<CardProspector> tableau;
    public List<CardProspector> discardPile;



	void Awake(){
		S = this;
	}

	void Start() {
		deck = GetComponent<Deck> ();
		deck.InitDeck (deckXML.text);

        layout = GetComponent<Layout>();
        layout.ReadLayout(layoutXML.text);

        drawPile = ConvertListCardsToListCardProspectors(deck.cards);

        LayoutGame();
	}

    CardProspector Draw()
    {

        CardProspector cd = drawPile[0]; // Pull the 0th CardProspector
        drawPile.RemoveAt(0);            // Then remove it from List<> drawPile
        return (cd);                      // And return it

    }

    void LayoutGame()
    {

        // Create an empty GameObject to serve as an anchor for the tableau // a

        if (layoutAnchor == null)
        {

            GameObject tGO = new GameObject("_LayoutAnchor");
            // ^ Create an empty GameObject named _LayoutAnchor in the Hierarchy

            layoutAnchor = tGO.transform;              // Grab its Transform
            layoutAnchor.transform.position = layoutCenter;   // Position it

        }



        CardProspector cp;

        // Follow the layout

        foreach (SlotDef tSD in layout.slotDefs)
        {

            cp = Draw(); 
            cp.faceUp = tSD.faceUp;
            cp.transform.parent = layoutAnchor; 

            cp.transform.localPosition = new Vector3(
                layout.multiplier.x * tSD.x,
                layout.multiplier.y * tSD.y,
                -tSD.layerID);

            cp.layoutID = tSD.id;
            cp.slotDef = tSD;

            cp.state = eCardState.tableau;

            cp.SetSortingLayerName(tSD.layerName);

            tableau.Add(cp); 
        }

    }

    List<CardProspector> ConvertListCardsToListCardProspectors(List<Card> lCD)
    {

        List<CardProspector> lCP = new List<CardProspector>();
        CardProspector tCP;

        foreach (Card tCD in lCD)
        {
            tCP = tCD as CardProspector;                                    
            lCP.Add(tCP);
        }

        return (lCP);

    }

}
