import React, { useEffect, useState } from "react";
import Point from "./Point"; // Make sure to import the Point component
import axios from "../../../axios"; // Make sure you've imported the API properly
import { useSelector } from "react-redux";
import { Link, useParams } from "react-router-dom";

export default function PointPanel() {
  const [points, setPoints] = useState([]);
  const [createMessage, setCreateMessage] = useState("");
  const [failCreateMessage, setFailCreateMessage] = useState("");
  const tokens = useSelector((state) => state.token);
  const { moduleId } = useParams();
  const [pointName, setPointName] = useState("");
  const [pointDescription, setPointDescription] = useState("");
  const { topicId } = useParams();

  async function fetchPoints() {
    try {
      const response = await axios.get(`/Point/All/${topicId}`, {
        headers: {
          Authorization: `Bearer ${tokens.accessToken}`,
        },
      });
      setPoints(response.data);
    } catch (error) {
      console.error("Error fetching points:", error);
    }
  }

  useEffect(() => {
    fetchPoints();
  }, []);

  const handleCreatePoint = async () => {
    console.log(moduleId);
    try {
      await axios.post(
        "/Point",
        { point1: pointName, description: pointDescription, moduleId: moduleId, topicId:topicId }, // Adjust topic name as needed
        {
          headers: {
            Authorization: `Bearer ${tokens.accessToken}`,
          },
        }
      );
      // After creating the topic, update the topic list
      fetchPoints();
      setCreateMessage("Topic created successfully!");
      setTimeout(() => {
        setCreateMessage("");
      }, 2000);
    } catch (error) {
      console.log("Error creating topic:", error);
      setFailCreateMessage(error.response.data.message);
      setTimeout(() => {
        setFailCreateMessage("");
      }, 2000);
    }
  };

  function savePointDescription(name) {
    setPointDescription(name);
  }
  function savePointName(name) {
    setPointName(name);
  }

  return (
    <>
      <div className="m-2">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><Link to="/ModulesManagment">Module Management</Link></li>
        <li class="breadcrumb-item"><Link to={`/ManageModule/${topicId}`} >Topic Management</Link></li>
        <li class="breadcrumb-item active">Point Management</li>
      </ol>
        <h1 className="text-center">Point Management Panel</h1>
        <div className="input-group mb-3">
          <span
            className="input-group-text btn btn-secondary"
            id="inputGroup-sizing-default"
            onClick={handleCreatePoint}
          >
            Create Point
          </span>
          <input
            type="text"
            className="form-control ms-2*-"
            aria-label="Sizing example input"
            aria-describedby="inputGroup-sizing-default"
            placeholder="Enter Point name"
            onBlur={(e) => savePointName(e.target.value)}
          />
          <input
            type="text"
            className="form-control ms-2"
            aria-label="Sizing example input"
            aria-describedby="inputGroup-sizing-default"
            placeholder="Enter Point Description"
            onBlur={(e) => savePointDescription(e.target.value)}
          />
        </div>
        {createMessage && (
          <div className="alert alert-success">{createMessage}</div>
        )}
        {failCreateMessage && (
          <div className="alert alert-danger">{failCreateMessage}</div>
        )}
        <h4>Point List</h4>
        <table className="table table-hover table-striped table-bordered">
          <thead>
            <tr>
              <th scope="col">Point Id</th>
              <th scope="col">Topic Id</th>
              <th scope="col">Point </th>
              <th scope="col">Description </th>
              <th scope="col">Created by</th>
              <th scope="col">Created Time</th>
              <th scope="col">Edit Point Name</th>
              <th scope="col">Edit Point Description</th>
              <th scope="col">Delete</th>
            </tr>
          </thead>
          <tbody>
            {points.map((point) => (
              <Point
                key={point.pointId}
                point={point}
                fetchPoints={fetchPoints}
              />
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}
