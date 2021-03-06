//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder v3.0.7.99
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.ModelsBuilder;
using Umbraco.ModelsBuilder.Umbraco;

namespace Umbraco.Web.PublishedContentModels
{
	// Mixin content Type 1093 with alias "accordion"
	/// <summary>Accordion</summary>
	public partial interface IAccordion : IPublishedContent
	{
		/// <summary>Items</summary>
		IEnumerable<IPublishedContent> Items { get; }
	}

	/// <summary>Accordion</summary>
	[PublishedContentModel("accordion")]
	public partial class Accordion : PublishedContentModel, IAccordion
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "accordion";
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

		public Accordion(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<Accordion, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Items: Add multiple items to be displayed in the accordion.
		///</summary>
		[ImplementPropertyType("items")]
		public IEnumerable<IPublishedContent> Items
		{
			get { return GetItems(this); }
		}

		/// <summary>Static getter for Items</summary>
		public static IEnumerable<IPublishedContent> GetItems(IAccordion that) { return that.GetPropertyValue<IEnumerable<IPublishedContent>>("items"); }
	}
}
