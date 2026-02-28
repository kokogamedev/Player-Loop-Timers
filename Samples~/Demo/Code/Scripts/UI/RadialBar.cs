using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using UnityRandom = UnityEngine.Random;

namespace PsigenVision.ImprovedTimers.Demo
{
	[UxmlElement]
	public partial class RadialBar: VisualElement
	{
		static CustomStyleProperty<Color> s_FillColor = new("--fill-color");
		static CustomStyleProperty<Color> s_BackgroundColor = new("--background-color");
		
		Color m_FillColor;
		Color m_BackgroundColor;
		
		//This is the number that the label displays as a percentage
		[SerializeField, DontCreateProperty] private float m_Progress;

		[UxmlAttribute, CreateProperty]
		public float progress
		{
			//The progress property is exposed in C#
			get => m_Progress;
			set
			{
				//Whenever the progress property changes, MarkDirtyRepaint() is named. This causes a call to the
				//generateVisualContents callback
				m_Progress = Mathf.Clamp(value, 0.05f, 100f);
				MarkDirtyRepaint();
			}
		}
		public RadialBar()
		{
			//Register a callback after custom style resolution
			RegisterCallback<CustomStyleResolvedEvent>(CustomStylesResolved);
			
			//Register a callback to generate the visual content of the radial bar
			generateVisualContent += GenerateVisualContent;
		}

		private void CustomStylesResolved(CustomStyleResolvedEvent evt)
		{
			//Receive a generic target from our UI system 
			//Check to see if the target is our current element (as their might be other elements also resolving their custom styles)
			if (evt.currentTarget == this)
			{
				RadialBar element = (RadialBar)evt.currentTarget;
				element.UpdateCustomStyles();
			}
		}

		private void UpdateCustomStyles()
		{
			bool repaint = false;
			//Sync our static values with local values (uss to C#)
			if (customStyle.TryGetValue(s_FillColor, out m_FillColor)) repaint = true;
			if (customStyle.TryGetValue(s_BackgroundColor, out m_BackgroundColor)) repaint = true;
			
			//If required, mark that the VisualElement requires repaint
			if (repaint) MarkDirtyRepaint();
		}

		private void GenerateVisualContent(MeshGenerationContext context)
		{
			//In charge of drawing our UI component (any mesh or object we want)
			
			//Step 1: Draw a basic circle
			
			//Grab width and height of the context rect
			float width = contentRect.width;
			float height = contentRect.height;

			var painter = context.painter2D; //cache reference to the 2D vector context
			
			//Call the draw arc (Arc) method
			painter.BeginPath();
			painter.lineWidth = 10f;
			//Draw a full arc from the center of our element around 360 degree angle 
			painter.Arc(new Vector2(width*0.5f, height*0.5f), width*0.5f, 0f, 360f);
			painter.ClosePath();
			painter.fillColor = m_BackgroundColor;
			painter.Fill();
			painter.Stroke();
			
			
			//Step 2: Add our fill element
			//Calculate our radial fill progress
			float amount = 360f * ((100f - m_Progress) / 100f); 
			
			//Call the draw arc (Arc) method
			painter.BeginPath();
			//Add a point, and start our path at the center instead
			painter.LineTo(new Vector2(width*0.5f, height*0.5f));
			painter.lineWidth = 10f;
			//Draw a full arc from the center of our element around 360 degree angle 
			painter.Arc(new Vector2(width*0.5f, height*0.5f), width*0.5f, 0.05f, 359.95f-amount); //set the angle slightly lower to see how it looks
			painter.ClosePath();
			painter.fillColor = m_FillColor;
			painter.Fill();
			painter.Stroke();
			
			//Draw another full arc overtop from the center of our element around 360 degree angle to add center black circle (visual purposes)
			painter.BeginPath();
			painter.Arc(new Vector2(width*0.5f, height*0.5f), width*0.4f, 0f, 360f);
			painter.ClosePath();
			painter.fillColor = Color.black;
			painter.Fill();
			painter.Stroke();
		}
	}
}