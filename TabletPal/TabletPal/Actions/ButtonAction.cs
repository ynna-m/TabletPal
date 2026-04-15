using System;
using System.Threading.Tasks;

namespace TabletPal.Actions
{
	public abstract class ButtonAction : IDisposable
	{
		public abstract void Invoke();

		public virtual void Dispose() {}
	}
}
