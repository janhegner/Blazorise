#region Using directives

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

#endregion

namespace Blazorise
{
    public partial class DateEdit<TValue> : BaseTextInput<TValue>
    {
        #region Members

        #endregion

        #region Methods

        public override async Task SetParametersAsync( ParameterView parameters )
        {
            await base.SetParametersAsync( parameters );

            if ( ParentValidation != null )
            {
                if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( DateExpression ), out var expression ) )
                    ParentValidation.InitializeInputExpression( expression );

                if ( parameters.TryGetValue<string>( nameof( Pattern ), out var pattern ) )
                {
                    // make sure we get the newest value
                    var value = parameters.TryGetValue<TValue>( nameof( Date ), out var inDate )
                        ? inDate
                        : InternalValue;

                    ParentValidation.InitializeInputPattern( pattern, value );
                }

                InitializeValidation();
            }
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.DateEdit() );
            builder.Append( ClassProvider.DateEditSize( Size ), Size != Size.None );
            builder.Append( ClassProvider.DateEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        protected override Task OnChangeHandler( ChangeEventArgs e )
        {
            if ( _HasFocus )
            {
                _ValueChangedCache = e?.Value?.ToString() ?? "";
                return Task.CompletedTask;
            }
            return CurrentValueHandler( e?.Value?.ToString() ?? "" );
        }

        protected async Task OnClickHandler( MouseEventArgs e )
        {
            if ( Disabled || ReadOnly )
                return;

            await JSRunner.ActivateDatePicker( ElementId, Parsers.InternalDateFormat );
        }

        protected override Task OnInternalValueChanged( TValue value )
        {
            return DateChanged.InvokeAsync( value );
        }

        protected override string FormatValueAsString( TValue value )
        {
            switch ( value )
            {
                case null:
                    return null;
                case DateTime datetime:
                    return datetime.ToString( Parsers.InternalDateFormat );
                case DateTimeOffset datetimeOffset:
                    return datetimeOffset.ToString( Parsers.InternalDateFormat );
                default:
                    throw new InvalidOperationException( $"Unsupported type {value.GetType()}" );
            }
        }

        protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
        {
            if ( Parsers.TryParseDate<TValue>( value, out var result ) )
            {
                return Task.FromResult( new ParseValue<TValue>( true, result, null ) );
            }
            else
            {
                return Task.FromResult( new ParseValue<TValue>( false, default, null ) );
            }
        }

        private async void _OnFocusIn( FocusEventArgs e )
        {
            _onFocusIn_impl();
            await FocusIn.InvokeAsync( e );
        }

        private void _onFocusIn_impl()
        {
            _ValueChangedCache = null;
            _HasFocus = true;
        }

        private async void _OnFocusOut( FocusEventArgs e )
        {
            _HasFocus = false;
            // fire changed event
            if ( _ValueChangedCache != null )
            {
                await CurrentValueHandler(_ValueChangedCache );
                _ValueChangedCache = null;
            }
            await FocusOut.InvokeAsync( e );
        }        
        
        private async void _OnFocus( FocusEventArgs e )
        {
            _onFocusIn_impl();
            await OnFocus.InvokeAsync( e );
        }

        #endregion

        #region Properties

        protected override TValue InternalValue { get => Date; set => Date = value; }

        /// <summary>
        /// Stores focus status of the input element
        /// </summary>
        /// ValueChanged events are suppressed during focus, since propagating the changed value back
        /// to the input element hinders a smooth user experience.
        private bool _HasFocus { get; set; } = false;

        /// <summary>
        /// Cache last changed value during focus
        /// </summary>
#nullable enable
        private string? _ValueChangedCache { get; set; } = null;
#nullable disable

        /// <summary>
        /// Gets or sets the input date value.
        /// </summary>
        [Parameter]
        public TValue Date { get; set; }

        /// <summary>
        /// Occurs when the date has changed.
        /// </summary>
        [Parameter] public EventCallback<TValue> DateChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the date value.
        /// </summary>
        [Parameter] public Expression<Func<TValue>> DateExpression { get; set; }

        /// <summary>
        /// The earliest date to accept.
        /// </summary>
        [Parameter] public DateTime? Min { get; set; }

        /// <summary>
        /// The latest date to accept.
        /// </summary>
        [Parameter] public DateTime? Max { get; set; }

        #endregion
    }
}