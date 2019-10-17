﻿using UnityEngine;
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

        MoveToTarget(Draw());

        UpdateDrawPile();

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

    void MoveToDiscard(CardProspector cd)
    {
        // Set the state of the card to discard
        cd.state = eCardState.discard;
        discardPile.Add(cd); // Add it to the discardPile List<>
        cd.transform.parent = layoutAnchor; // Update its transform parent

        // Position this card on the discardPile
        cd.transform.localPosition = new Vector3(
            layout.multiplier.x * layout.discardPile.x,
            layout.multiplier.y * layout.discardPile.y,
            -layout.discardPile.layerID + 0.5f);
        cd.faceUp = true;

        // Place it on top of the pile for depth sorting
        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(-100 + discardPile.Count);

    }



    // Make cd the new target card

    void MoveToTarget(CardProspector cd)
    {

        // If there is currently a target card, move it to discardPile

        if (target != null) MoveToDiscard(target);
        target = cd; // cd is the new target
        cd.state = eCardState.target;
        cd.transform.parent = layoutAnchor;


        // Move to the target position

        cd.transform.localPosition = new Vector3(
            layout.multiplier.x * layout.discardPile.x,
            layout.multiplier.y * layout.discardPile.y,
            -layout.discardPile.layerID);



        cd.faceUp = true; // Make it face-up
        // Set the depth sorting

        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(0);

    }



    // Arranges all the cards of the drawPile to show how many are left

    void UpdateDrawPile()
    {

        CardProspector cd;

        // Go through all the cards of the drawPile

        for (int i = 0; i < drawPile.Count; i++)
        {

            cd = drawPile[i];
            cd.transform.parent = layoutAnchor;

            // Position it correctly with the layout.drawPile.stagger

            Vector2 dpStagger = layout.drawPile.stagger;
            cd.transform.localPosition = new Vector3(

                layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x),
                layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y),
                -layout.drawPile.layerID + 0.1f * i);


            cd.faceUp = false; // Make them all face-down
            cd.state = eCardState.drawpile;

            // Set depth sorting
            cd.SetSortingLayerName(layout.drawPile.layerName);
            cd.SetSortOrder(-10 * i);
        }
    }

    public void CardClicked(CardProspector cd)
    {
        switch (cd.state)
        {

            case eCardState.target:

                // Clicking the target card does nothing

                break;



            case eCardState.drawpile:

                // Clicking any card in the drawPile will draw the next card

                MoveToDiscard(target); // Moves the target to the discardPile

                MoveToTarget(Draw());  // Moves the next drawn card to the target

                UpdateDrawPile();     // Restacks the drawPile

                break;





            case eCardState.tableau:

                // Clicking a card in the tableau will check if it's a valid play

                break;

        }
    }

}
