using System;
using System.Threading.Tasks;

namespace TabletPal.Actions
{
	public abstract class ButtonAction : IDisposable
	{
		public abstract Task Invoke();

		public virtual void Dispose() {}
	}
}
