using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus.Components {
	public class AttributesComponent : Component {
		private float _CurrentHealth = 100f;
		private float _MaxHealth = 100f;
		private float _Strength = 0;    //Used for damage.
		private float _StrModifier = 1f;
		private float _Dexterity = 0;   //used for defense.
		private float _DexModifier = 1f;
		private bool _IsDead = false;

		/// <summary>
		/// Gets an event called when the health of this Component runs out.
		/// </summary>
		public event Action<AttributesComponent> Died;
		/// <summary>
		/// Gets an event called when the amount of health this Component has remaining is changed.
		/// </summary>
		public event Action<AttributesComponent> CurrentHealthChanged;
		/// <summary>
		/// Gets an event called when the max health this Component has is changed.
		/// </summary>
		public event Action<AttributesComponent> MaxHealthChanged;

		/// <summary>
		/// Gets or sets the amount of health that this component has.
		/// </summary>
		public float CurrentHealth {
			get { return _CurrentHealth; }
			set {
				if(_IsDead && value > 0)
					throw new NotSupportedException("Unable to edit current health of a dead HealthComponent.");
				value = Math.Min(MaxHealth, Math.Max(value, 0));
				_CurrentHealth = value;
				if(this.CurrentHealthChanged != null)
					CurrentHealthChanged(this);
				if(CurrentHealth == 0) {
					_IsDead = true;
					if(this.Died != null)
						this.Died(this);
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum health allowed for this component.
		/// Setting this value also alters the current health to be equal to the old percentage of health.
		/// </summary>
		public float MaxHealth {
			get { return _MaxHealth; }
			set {
				if(_IsDead && value > 0)
					throw new NotSupportedException("Unable to edit max health of a dead HealthComponent.");
				float CurrPercent = CurrentHealth / MaxHealth;
				_MaxHealth = value;
				if(MaxHealthChanged != null)
					MaxHealthChanged(this);
				CurrentHealth = MaxHealth * CurrPercent;
			}
		}

		/// <summary>
		/// Gets or sets the strength.
		/// </summary>
		public float Strength {
			get { return _Strength; }
			set { _Strength = Math.Max(value, 0); }
		}

		/// <summary>
		/// Gets or sets the strength modifer. Should be expressed as a percentage.
		/// </summary>
		public float StrModifier {
			get { return _StrModifier; }
			set { _StrModifier = Math.Max(value, 0); }
		}

		/// <summary>
		/// Gets or sets the Dexterity.
		/// </summary>
		public float Dexterity {
			get { return _Dexterity; }
			set { _Dexterity = Math.Max(value, 0); }
		}

		/// <summary>
		/// Gets or sets the dexterity modifier. Should be expressed as a percentage.
		/// </summary>
		public float DexModifier {
			get { return _DexModifier; }
			set { _DexModifier = Math.Max(value, 0); }
		}

		/// <summary>
		/// Gets the overall strength.
		/// </summary>
		public float Attack { get { return Strength * StrModifier; } }
		/// <summary>
		/// Gets the overall defense.
		/// </summary>
		public float Defense { get { return Dexterity * DexModifier; } }
	}
}