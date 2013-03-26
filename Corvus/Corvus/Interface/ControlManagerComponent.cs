using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corvus.Interface.Controls;
using CorvEngine;
using CorvEngine.Scenes;
using CorvEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus.Interface
{
    /// <summary>
    /// A GameStateComponent to manage menu controls.
    /// </summary>
    public class ControlManagerComponent : GameStateComponent
    {
        private List<BaseControl> _Controls = new List<BaseControl>();
        private int _CurrentIndex = 0;

        /// <summary>
        /// Adds a control.
        /// </summary>
        public void AddControl(BaseControl bc)
        {
            _Controls.Add(bc);
        }

        /// <summary>
        /// Sets focus to the first focusable element.
        /// </summary>
        public void SetFocus()
        {
            foreach (var bc in _Controls)
            {
                if (bc.TabStop && bc.IsEnabled)
                {
                    bc.HasFocus = true;
                    _CurrentIndex = _Controls.IndexOf(bc);
                    break;
                }
            }
        }

        /// <summary>
        /// Calls the selected event for the focused control.
        /// </summary>
        /// <param name="state"></param>
        public void SelectControl(BindState state)
        {
            if (_Controls.Count == 0 || state == BindState.Released) //TODO: Get rid of this when i figure out how to do menu binds.
                return;

            _Controls[_CurrentIndex].OnSelected();
        }

        /// <summary>
        /// Moves to the next focusable element.
        /// </summary>
        public void NextControl(BindState state)
        {
            if (_Controls.Count == 0 || state == BindState.Released) //TODO: Get rid of this when i figure out how to do menu binds.
                return;

            int currentIndex = _CurrentIndex;
            _Controls[_CurrentIndex].HasFocus = false;

            do
            {
                _CurrentIndex++;
                if (_CurrentIndex == _Controls.Count)
                    _CurrentIndex = 0;

                if (_Controls[_CurrentIndex].TabStop && _Controls[_CurrentIndex].IsEnabled)
                    break;

            } while (currentIndex != _CurrentIndex);

            _Controls[_CurrentIndex].HasFocus = true;
        }

        /// <summary>
        /// Moves to the previous focusable element.
        /// </summary>
        public void PreviousControl(BindState state)
        {
            if (_Controls.Count == 0 || state == BindState.Released) //TODO: Get rid of this when i figure out how to do menu binds.
                return;

            int currentIndex = _CurrentIndex;
            _Controls[_CurrentIndex].HasFocus = false;

            do
            {
                _CurrentIndex--;
                if (_CurrentIndex < 0)
                    _CurrentIndex = _Controls.Count - 1;

                if (_Controls[_CurrentIndex].TabStop && _Controls[_CurrentIndex].IsEnabled)
                    break;

            } while (currentIndex != _CurrentIndex);

            _Controls[_CurrentIndex].HasFocus = true;
        }

        public ControlManagerComponent(GameState state)
            : base(state)
        {
            this.Enabled = true;
            this.Visible = true;
        }

        protected override void OnUpdate(Microsoft.Xna.Framework.GameTime Time)
        {
            foreach (var bc in _Controls)
            {
                if (bc.IsEnabled)
                    bc.Update(Time);
            }
        }

        protected override void OnDraw(Microsoft.Xna.Framework.GameTime Time)
        {
            foreach (var bc in _Controls)
            {
                if (bc.IsVisible)
                    bc.Draw(Time);
            }
        }


    }
}
