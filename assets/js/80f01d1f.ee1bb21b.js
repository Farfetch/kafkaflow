"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[7855],{3905:(e,r,n)=>{n.d(r,{Zo:()=>u,kt:()=>s});var a=n(7294);function t(e,r,n){return r in e?Object.defineProperty(e,r,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[r]=n,e}function o(e,r){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);r&&(a=a.filter((function(r){return Object.getOwnPropertyDescriptor(e,r).enumerable}))),n.push.apply(n,a)}return n}function l(e){for(var r=1;r<arguments.length;r++){var n=null!=arguments[r]?arguments[r]:{};r%2?o(Object(n),!0).forEach((function(r){t(e,r,n[r])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):o(Object(n)).forEach((function(r){Object.defineProperty(e,r,Object.getOwnPropertyDescriptor(n,r))}))}return e}function d(e,r){if(null==e)return{};var n,a,t=function(e,r){if(null==e)return{};var n,a,t={},o=Object.keys(e);for(a=0;a<o.length;a++)n=o[a],r.indexOf(n)>=0||(t[n]=e[n]);return t}(e,r);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(a=0;a<o.length;a++)n=o[a],r.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(t[n]=e[n])}return t}var i=a.createContext({}),p=function(e){var r=a.useContext(i),n=r;return e&&(n="function"==typeof e?e(r):l(l({},r),e)),n},u=function(e){var r=p(e.components);return a.createElement(i.Provider,{value:r},e.children)},f={inlineCode:"code",wrapper:function(e){var r=e.children;return a.createElement(a.Fragment,{},r)}},c=a.forwardRef((function(e,r){var n=e.components,t=e.mdxType,o=e.originalType,i=e.parentName,u=d(e,["components","mdxType","originalType","parentName"]),c=p(n),s=t,y=c["".concat(i,".").concat(s)]||c[s]||f[s]||o;return n?a.createElement(y,l(l({ref:r},u),{},{components:n})):a.createElement(y,l({ref:r},u))}));function s(e,r){var n=arguments,t=r&&r.mdxType;if("string"==typeof e||t){var o=n.length,l=new Array(o);l[0]=c;var d={};for(var i in r)hasOwnProperty.call(r,i)&&(d[i]=r[i]);d.originalType=e,d.mdxType="string"==typeof e?e:t,l[1]=d;for(var p=2;p<o;p++)l[p]=n[p];return a.createElement.apply(null,l)}return a.createElement.apply(null,n)}c.displayName="MDXCreateElement"},9351:(e,r,n)=>{n.r(r),n.d(r,{assets:()=>i,contentTitle:()=>l,default:()=>f,frontMatter:()=>o,metadata:()=>d,toc:()=>p});var a=n(7462),t=(n(7294),n(3905));const o={},l="TypedHandlerConfigurationBuilder.WhenNoHandlerFound method",d={unversionedId:"reference/KafkaFlow.TypedHandler/KafkaFlow.TypedHandler/TypedHandlerConfigurationBuilder/WhenNoHandlerFound",id:"reference/KafkaFlow.TypedHandler/KafkaFlow.TypedHandler/TypedHandlerConfigurationBuilder/WhenNoHandlerFound",title:"TypedHandlerConfigurationBuilder.WhenNoHandlerFound method",description:"Register the action to be executed when no handler was found to process the message",source:"@site/docs/reference/KafkaFlow.TypedHandler/KafkaFlow.TypedHandler/TypedHandlerConfigurationBuilder/WhenNoHandlerFound.md",sourceDirName:"reference/KafkaFlow.TypedHandler/KafkaFlow.TypedHandler/TypedHandlerConfigurationBuilder",slug:"/reference/KafkaFlow.TypedHandler/KafkaFlow.TypedHandler/TypedHandlerConfigurationBuilder/WhenNoHandlerFound",permalink:"/kafkaflow/docs/reference/KafkaFlow.TypedHandler/KafkaFlow.TypedHandler/TypedHandlerConfigurationBuilder/WhenNoHandlerFound",draft:!1,editUrl:"https://github.com/farfetch/kafkaflow/tree/master/website/docs/reference/KafkaFlow.TypedHandler/KafkaFlow.TypedHandler/TypedHandlerConfigurationBuilder/WhenNoHandlerFound.md",tags:[],version:"current",frontMatter:{},sidebar:"tutorialSidebar",previous:{title:"TypedHandlerConfigurationBuilder.AddHandlersFromAssemblyOf method (1 of 2)",permalink:"/kafkaflow/docs/reference/KafkaFlow.TypedHandler/KafkaFlow.TypedHandler/TypedHandlerConfigurationBuilder/AddHandlersFromAssemblyOf"},next:{title:"TypedHandlerConfigurationBuilder.WithHandlerLifetime method",permalink:"/kafkaflow/docs/reference/KafkaFlow.TypedHandler/KafkaFlow.TypedHandler/TypedHandlerConfigurationBuilder/WithHandlerLifetime"}},i={},p=[{value:"See Also",id:"see-also",level:2}],u={toc:p};function f(e){let{components:r,...n}=e;return(0,t.kt)("wrapper",(0,a.Z)({},u,n,{components:r,mdxType:"MDXLayout"}),(0,t.kt)("h1",{id:"typedhandlerconfigurationbuilderwhennohandlerfound-method"},"TypedHandlerConfigurationBuilder.WhenNoHandlerFound method"),(0,t.kt)("p",null,"Register the action to be executed when no handler was found to process the message"),(0,t.kt)("pre",null,(0,t.kt)("code",{parentName:"pre",className:"language-csharp"},"public TypedHandlerConfigurationBuilder WhenNoHandlerFound(Action<IMessageContext> handler)\n")),(0,t.kt)("table",null,(0,t.kt)("thead",{parentName:"table"},(0,t.kt)("tr",{parentName:"thead"},(0,t.kt)("th",{parentName:"tr",align:null},"parameter"),(0,t.kt)("th",{parentName:"tr",align:null},"description"))),(0,t.kt)("tbody",{parentName:"table"},(0,t.kt)("tr",{parentName:"tbody"},(0,t.kt)("td",{parentName:"tr",align:null},"handler"),(0,t.kt)("td",{parentName:"tr",align:null},"The handler that will be executed")))),(0,t.kt)("h2",{id:"see-also"},"See Also"),(0,t.kt)("ul",null,(0,t.kt)("li",{parentName:"ul"},"class\xa0",(0,t.kt)("a",{parentName:"li",href:"/kafkaflow/docs/reference/KafkaFlow.TypedHandler/KafkaFlow.TypedHandler/TypedHandlerConfigurationBuilder/"},"TypedHandlerConfigurationBuilder")),(0,t.kt)("li",{parentName:"ul"},"namespace\xa0",(0,t.kt)("a",{parentName:"li",href:"/kafkaflow/docs/reference/KafkaFlow.TypedHandler/"},"KafkaFlow.TypedHandler"))))}f.isMDXComponent=!0}}]);