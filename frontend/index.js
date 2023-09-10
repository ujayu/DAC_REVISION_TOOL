import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';

import {createBrowserRouter, RouterProvider } from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min';
import "bootswatch/dist/vapor/bootstrap.min.css";
import './App.css';
import store from './app/store'; 
import { Provider } from 'react-redux'

import axios from 'axios';
import Layout from './component/layout/Layout';
import Home from './component/middle/Home';
import Modules from './component/middle/Modules';
import Feedback from './component/middle/Feedback';
import About from './component/middle/About';
import Login from './component/Login';
import Register from './component/Register';
import Module from './component/middle/Module';
import Point from './component/middle/Point';
import Profile from './component/Profile';
import Admin from './component/middle/Admin';
import ModulesManagment from './component/middle/ModulesManagment';
import TopicPanel from './component/middle/ModulesManagment/TopicPanel';
import PointPanel from './component/middle/ModulesManagment/PointPanel';
import EmailVerify from './src/component/middle/EmailVerify';

const profileLoader = async (accessToken) => {
  try {
    // Make an API call to fetch user data
    const response = await axios.get('/User', {
      headers: {
        Authorization: `Bearer ${accessToken}` // Use the token from the response
      }
    });

    if (response.data.isSuccess) {
      return { user: response.data.data }; // Pass the user data as a prop
    }
  } catch (error) {
    console.error('Error fetching user data:', error);
    return null;
  }
};

const router  = createBrowserRouter([
  {
    path: "/",
    element: <Layout />,
    children: [
      {
        path: '',
        element: <Home />,
      },
      {
        path: 'home',
        element: <Home />,
      },
      {
        path: 'modules',
        element: <Modules />,
      },
      {
        path: 'feedback',
        element: <Feedback />,
      },
      {
        path: 'about',
        element: <About />,
      },
      {
        path: 'login',
        element: <Login />,
      },
      {
        path: 'register',
        element: <Register />,
      },
      {
        path: 'profile',
        element: <Profile/>, 
        loader: profileLoader,
      },
      {
        path: 'module/:moduleId',
        element: <Module />,
      },
      {
        path: 'Point/:pointId',
        element: <Point />,
      },
      {
        path: 'admin',
        element: <Admin />,
      },
      {
        path: "modulesManagment",
        element: <ModulesManagment />,
      },
      {
        path: "ManageModule/:moduleId",
        element: <TopicPanel />,
      },
      {

        path: "ManagePoint/:topicId",
        element: <PointPanel />,
      },
      {
        path: "emailVerify/:action",
        element: <EmailVerify />,
      },
      {
        path: '*',
        element: <Home />,
        loader: homeLoader,
      }, 
    ],
  },
]);


ReactDOM.createRoot(document.getElementById("root")).render(
  <Provider store={store}>
      <RouterProvider router={router} />  
  </Provider>
);
