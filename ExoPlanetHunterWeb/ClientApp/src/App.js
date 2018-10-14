import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';

export default class App extends Component {
  displayName = App.name

  render() {
    return (
      <Layout>
        <Route exact path='app/' component={Home} />
        <Route path='app/counter' component={Counter} />
        <Route path='app/fetchdata' component={FetchData} />
      </Layout>
    );
  }
}
