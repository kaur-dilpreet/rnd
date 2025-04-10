/**
 * Highstock JS v11.3.0 (2024-01-10)
 *
 * Indicator series type for Highcharts Stock
 *
 * (c) 2010-2024 Daniel Studencki
 *
 * License: www.highcharts.com/license
 */!function(t){"object"==typeof module&&module.exports?(t.default=t,module.exports=t):"function"==typeof define&&define.amd?define("highcharts/indicators/price-channel",["highcharts","highcharts/modules/stock"],function(e){return t(e),t.Highcharts=e,t}):t("undefined"!=typeof Highcharts?Highcharts:void 0)}(function(t){"use strict";var e=t?t._modules:{};function o(t,e,o,i){t.hasOwnProperty(e)||(t[e]=i.apply(null,o),"function"==typeof CustomEvent&&window.dispatchEvent(new CustomEvent("HighchartsModuleLoaded",{detail:{path:e,module:t[e]}})))}o(e,"Stock/Indicators/ArrayUtilities.js",[],function(){return{getArrayExtremes:function(t,e,o){return t.reduce(function(t,i){return[Math.min(t[0],i[e]),Math.max(t[1],i[o])]},[Number.MAX_VALUE,-Number.MAX_VALUE])}}}),o(e,"Stock/Indicators/MultipleLinesComposition.js",[e["Core/Series/SeriesRegistry.js"],e["Core/Utilities.js"]],function(t,e){var o,i=t.seriesTypes.sma.prototype,n=e.defined,r=e.error,a=e.merge;return function(t){var e=["bottomLine"],o=["top","bottom"],s=["top"];function p(t){return"plot"+t.charAt(0).toUpperCase()+t.slice(1)}function l(t,e){var o=[];return(t.pointArrayMap||[]).forEach(function(t){t!==e&&o.push(p(t))}),o}function c(){var t,e=this,o=e.pointValKey,s=e.linesApiNames,c=e.areaLinesNames,h=e.points,u=e.options,f=e.graph,d={options:{gapSize:u.gapSize}},y=[],m=l(e,o),g=h.length;if(m.forEach(function(e,o){for(y[o]=[];g--;)t=h[g],y[o].push({x:t.x,plotX:t.plotX,plotY:t[e],isNull:!n(t[e])});g=h.length}),e.userOptions.fillColor&&c.length){var v=y[m.indexOf(p(c[0]))],A=1===c.length?h:y[m.indexOf(p(c[1]))],C=e.color;e.points=A,e.nextPoints=v,e.color=e.userOptions.fillColor,e.options=a(h,d),e.graph=e.area,e.fillGraph=!0,i.drawGraph.call(e),e.area=e.graph,delete e.nextPoints,delete e.fillGraph,e.color=C}s.forEach(function(t,o){y[o]?(e.points=y[o],u[t]?e.options=a(u[t].styles,d):r('Error: "There is no '+t+' in DOCS options declared. Check if linesApiNames are consistent with your DOCS line names."'),e.graph=e["graph"+t],i.drawGraph.call(e),e["graph"+t]=e.graph):r('Error: "'+t+" doesn't have equivalent in pointArrayMap. To many elements in linesApiNames relative to pointArrayMap.\"")}),e.points=h,e.options=u,e.graph=f,i.drawGraph.call(e)}function h(t){var e,o=[],n=[];if(t=t||this.points,this.fillGraph&&this.nextPoints){if((e=i.getGraphPath.call(this,this.nextPoints))&&e.length){e[0][0]="L",o=i.getGraphPath.call(this,t),n=e.slice(0,o.length);for(var r=n.length-1;r>=0;r--)o.push(n[r])}}else o=i.getGraphPath.apply(this,arguments);return o}function u(t){var e=[];return(this.pointArrayMap||[]).forEach(function(o){e.push(t[o])}),e}function f(){var t,e=this,o=this.pointArrayMap,n=[];n=l(this),i.translate.apply(this,arguments),this.points.forEach(function(i){o.forEach(function(o,r){t=i[o],e.dataModify&&(t=e.dataModify.modifyValue(t)),null!==t&&(i[n[r]]=e.yAxis.toPixels(t,!0))})})}t.compose=function(t){var i=t.prototype;return i.linesApiNames=i.linesApiNames||e.slice(),i.pointArrayMap=i.pointArrayMap||o.slice(),i.pointValKey=i.pointValKey||"top",i.areaLinesNames=i.areaLinesNames||s.slice(),i.drawGraph=c,i.getGraphPath=h,i.toYData=u,i.translate=f,t}}(o||(o={})),o}),o(e,"Stock/Indicators/PC/PCIndicator.js",[e["Stock/Indicators/ArrayUtilities.js"],e["Stock/Indicators/MultipleLinesComposition.js"],e["Core/Color/Palettes.js"],e["Core/Series/SeriesRegistry.js"],e["Core/Utilities.js"]],function(t,e,o,i,n){var r,a=this&&this.__extends||(r=function(t,e){return(r=Object.setPrototypeOf||({__proto__:[]})instanceof Array&&function(t,e){t.__proto__=e}||function(t,e){for(var o in e)Object.prototype.hasOwnProperty.call(e,o)&&(t[o]=e[o])})(t,e)},function(t,e){if("function"!=typeof e&&null!==e)throw TypeError("Class extends value "+String(e)+" is not a constructor or null");function o(){this.constructor=t}r(t,e),t.prototype=null===e?Object.create(e):(o.prototype=e.prototype,new o)}),s=i.seriesTypes.sma,p=n.merge,l=n.extend,c=function(e){function i(){return null!==e&&e.apply(this,arguments)||this}return a(i,e),i.prototype.getValues=function(e,o){var i,n,r,a,s,p,l,c=o.period,h=e.xData,u=e.yData,f=u?u.length:0,d=[],y=[],m=[];if(!(f<c)){for(l=c;l<=f;l++)a=h[l-1],s=u.slice(l-c,l),i=((n=(p=t.getArrayExtremes(s,2,1))[1])+(r=p[0]))/2,d.push([a,n,i,r]),y.push(a),m.push([n,i,r]);return{values:d,xData:y,yData:m}}},i.defaultOptions=p(s.defaultOptions,{params:{index:void 0,period:20},lineWidth:1,topLine:{styles:{lineColor:o.colors[2],lineWidth:1}},bottomLine:{styles:{lineColor:o.colors[8],lineWidth:1}},dataGrouping:{approximation:"averages"}}),i}(s);return l(c.prototype,{areaLinesNames:["top","bottom"],nameBase:"Price Channel",nameComponents:["period"],linesApiNames:["topLine","bottomLine"],pointArrayMap:["top","middle","bottom"],pointValKey:"middle"}),e.compose(c),i.registerSeriesType("pc",c),c}),o(e,"masters/indicators/price-channel.src.js",[],function(){})});