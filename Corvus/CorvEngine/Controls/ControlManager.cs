using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

/*
 * There is bound to be dozens of bugs :P
 */
namespace CorvEngine.Controls
{
    /// <summary>
    /// Event Arguments used in ControlManager.
    /// </summary>
    public class ControlManagerEventArgs : EventArgs
    {
        /// <summary>
        /// The Element Being affected.
        /// </summary>
        public UIElement Element { get; set; }

        /// <summary>
        /// Creates a new instnce of ControlManagerEventArgs.
        /// </summary>
        public ControlManagerEventArgs(UIElement element)
        {
            this.Element = element;
        }
    }

    /// <summary>
    /// A class to manage all controls.
    /// </summary>
    public class ControlManager //: List<UIElement>
    {
        private List<UIElement> _ElementCollection; //contains all the elements
        private List<UIElement> _FocusableElements; //contains only the focusable elements
        private bool _AcceptsInput = true;
        private int _CurrentElementIndex = 0;
        private UpdateQueue _UpdateQueue = new UpdateQueue();
        
        /// <summary>
        /// Fires when there is a new element being added to the ControlManager.
        /// </summary>
        public event EventHandler<ControlManagerEventArgs> UpdateAdd;

        /// <summary>
        /// Fires when there is a element being removed from the ControlManager.
        /// </summary>
        public event EventHandler<ControlManagerEventArgs> UpdateRemove;

        /// <summary>
        /// Determines if the ControlManager should take input from the user.
        /// </summary>
        public bool AcceptsInput
        {
            get { return _AcceptsInput; }
            set { _AcceptsInput = value; }
        }

        /// <summary>
        /// Instantiates a new instance of ControlManager.
        /// </summary>
        public ControlManager()
            :base()
        {
            _ElementCollection = new List<UIElement>();
            _FocusableElements = new List<UIElement>();
        }

        /// <summary>
        /// Updates the elements.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (_ElementCollection.Count == 0)
                return;

            foreach (UIElement element in _ElementCollection)
            {
                if (element.IsEnabled)
                    element.Update(gameTime);
            }

            //applies updates
            if (_UpdateQueue.Count != 0)
            {
                foreach (QueueType e in _UpdateQueue.Queue)
                {
                    if (e.Command == QueueCommand.Add)
                    {
                        Add(e.Element);
                        if (UpdateAdd != null)
                            UpdateAdd(this, new ControlManagerEventArgs(e.Element));
                    }
                    else if (e.Command == QueueCommand.Remove)
                    {
                        Remove(e.Element);
                        if (UpdateRemove != null)
                            UpdateRemove(this, new ControlManagerEventArgs(e.Element));
                    }
                }
                _UpdateQueue.Clear();
            }

            if (!_AcceptsInput)
                return;

            if (InputHandler.KeyPressed(Keys.Down))
                NextControl();
            if (InputHandler.KeyPressed(Keys.Up))
                PreviousControl();
        }

        /// <summary>
        /// Draws the elements.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (UIElement element in _ElementCollection)
            {
                if (element.IsVisible)
                    element.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Sets the focus to the first item.
        /// </summary>
        public void SetFocus()
        {
            foreach (UIElement ui in _FocusableElements)
                ui.HasFocus = false;

            var e = _FocusableElements.First();
            if (e != null)
            {
                e.HasFocus = true;
                _CurrentElementIndex = _FocusableElements.IndexOf(e);
            }
        }

        /// <summary>
        /// Sets the focus based on the specified element.
        /// </summary>
        public void SetFocus(UIElement element)
        {
            foreach (UIElement ui in _FocusableElements)
                ui.HasFocus = false;

            var e = _FocusableElements.Where(c => c == element).SingleOrDefault();
            if (e != null)
            {
                e.HasFocus = true;
                _CurrentElementIndex = _FocusableElements.IndexOf(e);
            }
        }
        
        /// <summary>
        /// Addes an element. Note: Doesn't add elements into stackpanels, etc.
        /// </summary>
        public void Add(UIElement element)
        {
            _ElementCollection.Add(element);

            List<UIElement> focusable = GetFocusableElements(new List<UIElement>() { element });
            foreach (UIElement e in focusable)
                _FocusableElements.Add(e);
        }

        /// <summary>
        /// Removes an element. Note: Doesn't remove elements within stackpanels, etc. 
        /// </summary>
        public void Remove(UIElement element)
        {
            _ElementCollection.Remove(element);

            List<UIElement> focusable = GetFocusableElements(new List<UIElement>() { element });
            foreach (UIElement e in focusable)
                _FocusableElements.Remove(e);

        }

        /// <summary>
        /// Add/Remove elements dynamically. 
        /// </summary>
        public void Push(UIElement element, QueueCommand command)
        {
            _UpdateQueue.Enqueue(element, command);
        }

        /// <summary>
        /// Gets the element with the specified name. Returns null if nothing is found.
        /// </summary>
        public UIElement GetElementByName(string name)
        {
            return GetElementByName(_ElementCollection, name);
        }

        /// <summary>
        /// Recursively searches for the element with the specified name.
        /// </summary>
        private UIElement GetElementByName(List<UIElement> elements, string name)
        {
            UIElement element = null;
            foreach (UIElement e in elements)
            {
                if (e.Name.Equals(name))
                    element = e;
                else if (e is Panel)
                    element = GetElementByName(((Panel)e).Items, name);
                else if (e is UserControl)
                    element = GetElementByName(new List<UIElement>() { ((UserControl)e).Content }, name);
            }
            return element;
        }

        /// <summary>
        /// Recursively gets all the focusable elements.
        /// </summary>
        private List<UIElement> GetFocusableElements(List<UIElement> elements)
        {
            List<UIElement> output = new List<UIElement>();
            foreach (UIElement e in elements)
            {
                if (e is Panel)
                {
                    foreach (UIElement p in GetFocusableElements(((Panel)e).Items))
                        output.Add(p);
                }
                else if (e is UserControl)
                {
                    foreach (UIElement p in GetFocusableElements(new List<UIElement>() { ((UserControl)e).Content }))
                        output.Add(p);
                }
                else if (e.TabStop)//possibly will need to consider more later 
                    output.Add(e);
            }
            return output;
        }

        /// <summary>
        /// Moves to the next focusable element.
        /// </summary>
        private void NextControl()
        {
            if (_ElementCollection.Count == 0 || _FocusableElements.Count == 0)
                return;

            int currentIndex = _CurrentElementIndex;
            _FocusableElements[_CurrentElementIndex].HasFocus = false;

            do
            {
                _CurrentElementIndex++;
                if (_CurrentElementIndex == _FocusableElements.Count)
                    _CurrentElementIndex = 0;

                if (_FocusableElements[_CurrentElementIndex].TabStop && _FocusableElements[_CurrentElementIndex].IsEnabled)
                    break;

            } while (currentIndex != _CurrentElementIndex);

            _FocusableElements[_CurrentElementIndex].HasFocus = true;
        }

        /// <summary>
        /// Moves to the previous focusable element.
        /// </summary>
        private void PreviousControl()
        {
            if (_ElementCollection.Count == 0 || _FocusableElements.Count == 0)
                return;

            int currentIndex = _CurrentElementIndex;
            _FocusableElements[_CurrentElementIndex].HasFocus = false;

            do
            {
                _CurrentElementIndex--;
                if (_CurrentElementIndex < 0)
                    _CurrentElementIndex = _FocusableElements.Count - 1;

                if (_FocusableElements[_CurrentElementIndex].TabStop && _FocusableElements[_CurrentElementIndex].IsEnabled)
                    break;

            } while (currentIndex != _CurrentElementIndex);

            _FocusableElements[_CurrentElementIndex].HasFocus = true;
        }


        #region Update Queue
        /// <summary>
        /// Determines action to take when adding elements dynamically.
        /// </summary>
        public enum QueueCommand
        { 
            /// <summary>
            /// Indicates the queue should add the element.
            /// </summary>
            Add, 
            /// <summary>
            /// Indicates the queue should remove the element.
            /// </summary>
            Remove 
        }

        /// <summary>
        /// A queue to manage updates for the ControlManager.
        /// </summary>
        private class UpdateQueue
        {
            private Queue<QueueType> _Queue = new Queue<QueueType>();

            /// <summary>
            /// Gets the queue.
            /// </summary>
            public Queue<QueueType> Queue { get { return _Queue; } }

            /// <summary>
            /// Gets the number of items in the queue.
            /// </summary>
            public int Count { get { return _Queue.Count; } }

            /// <summary>
            /// Instantiates a new UpdateQueue.
            /// </summary>
            public void Enqueue(UIElement element, QueueCommand command)
            {
                _Queue.Enqueue(new QueueType(element, command));
            }

            /// <summary>
            /// Removes the first item from the queue and returns it.
            /// </summary>
            public QueueType Dequeue()
            {
                return _Queue.Dequeue();
            }

            /// <summary>
            /// Clear the queue.
            /// </summary>
            public void Clear()
            {
                _Queue.Clear();
            }

        }

        /// <summary>
        /// A class for holding additional data for elements.
        /// </summary>
        private class QueueType
        {
            /// <summary>
            /// The element.
            /// </summary>
            public UIElement Element { get; set; }
            /// <summary>
            /// The action to take.
            /// </summary>
            public QueueCommand Command { get; set; }

            /// <summary>
            /// Instantiates a new instance of QueueType.
            /// </summary>
            public QueueType(UIElement element, QueueCommand command)
            {
                Element = element;
                Command = command;
            }
        }
        #endregion
    }

    
}
