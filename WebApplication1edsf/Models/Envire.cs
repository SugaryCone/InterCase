using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1edsf.Models
{
	internal class Envire
	{
		Envire enclosing;



        private Dictionary<String, Object> values = new Dictionary<String, Object>();

		public Envire()
		{
			enclosing = null;
		}

		public Envire(Envire enclosing)
		{
			this.enclosing = enclosing;
		}

		public void define(String name, Object value)
		{
			if(!values.TryAdd(name, value))
			{
				values[name] = value;
			}
		}

		public Object get(Token name)
		{

			return get(name.lexeme);
		}
		public Object get(string name)
		{

			if (values.ContainsKey(name))
			{
				return values[name];
			}
			if (enclosing != null) return enclosing.get(name);
			throw new RuntimeError(name,
				"Undefined variable '" + name + "'.");
		}
		public void assign(Token name, Object value)
		{
			if (values.ContainsKey(name.lexeme))
			{
				values[name.lexeme] = value;
				return;
			}
			if (enclosing != null)
			{
				enclosing.assign(name, value);
				return;
			}
			throw new RuntimeError(name,
				"Undefined variable '" + name.lexeme + "'.");
		}
	}
}
