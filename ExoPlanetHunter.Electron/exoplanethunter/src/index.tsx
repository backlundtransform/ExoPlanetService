
import 'react-app-polyfill/ie11';

import *  as ReactDOM from 'react-dom'
import  * as React from "react"
import "./App.css"

import * as serviceWorker from './serviceWorker';

import Navigate from './components/Navigate'

ReactDOM.render(
    <Navigate/>,
    document.getElementById('root')
  );

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
