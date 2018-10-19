import React, { Component } from 'react';
import { BrowserRouter as Router,Route, Link } from 'react-router-dom';

import { Home } from './Home';
import { Topics  } from './Topics';

export default class Navigate extends Component {


  render() {
    return (
        <Router>
        <div>
          <ul>
            <li>
              <Link to="/home">Home</Link>
            </li>
            <li>
              <Link to="/topics">Topics</Link>
            </li>
          </ul>
    
          <hr />
          <Route exact path="/home" component={Home} />
          <Route exact path="/topics" component={Topics} />
        </div>
      </Router>
    );
  }
}