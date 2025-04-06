﻿using LD57.Interaction;
using LD57.LevelManagement;
using LD57.UiSystem;
using Myra.Graphics2D.UI;

namespace LD57.Puzzle
{
    public class Hole : SyncScript, IInteractable
    {
        public UiCanvas Canvas { get; set; }
        public TheBigObject Target { get; set; }
        public string NextLevel { get; set; }

        HoleWidget ui;

        public bool Interactable => Target != null &&
            (Target.Index == Target.States.Count - 1 || Target.States.Count == 0);
        [DataMemberIgnore] public bool Focused { get; set; }

        public void Interact()
        {
            Canvas.UiEnabled = true;
            //TODO: Play Animation
        }

        public void Interact2() =>
            Interact();

        public override void Start()
        {
            ui = new HoleWidget(Services.GetService<UiManager>());
            Canvas.Root = ui;
        }

        public override void Update()
        {
            if (Target != null)
            {
                
            }

            if (Canvas.UiEnabled && Canvas.HasInputFocus)
            {
                if (Input.IsKeyPressed(Keys.E))
                    LoadNextLevel();
            }
        }

        public void LoadNextLevel()
        {
            Services.GetService<LevelManager>().LoadLevel(NextLevel);
        }

        public class HoleWidget : Grid
        {
            public HoleWidget(UiManager manager)
            {
                RowsProportions.Add(new Proportion(ProportionType.Fill));
                RowsProportions.Add(new Proportion(ProportionType.Part, 0.25f));

                var cont = new Label()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "PRESS [E] TO CONTINUE",
                    Font = manager.Fonts.CaviarDreamsBold.GetFont(32),
                };

                SetRow(cont, 1);
                Widgets.Add(cont);
            }
        }
    }
}
