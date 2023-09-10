import React, { useEffect, useState } from "react";
import Topic from "./Topic"; // Make sure to import the Topic component
import axios from "./../../../axios"; // Make sure you've imported the API properly
import { useSelector } from "react-redux";
import { Link, useParams } from "react-router-dom";

export default function TopicsManagment() {
  const [topics, setTopics] = useState([]); // Define state to hold topics data
  const [createMessage, setCreateMessage] = useState("");
  const [failCreateMessage, setFailCreateMessage] = useState("");
  const [topicName, setTopicName] = useState("");
  const tokens = useSelector((state) => state.token);
  const { moduleId } = useParams();

  // Fetch topics data when component mounts
  async function fetchTopics() {
    try {
      const response = await axios.get(`/Topic/All/${moduleId}`, {
        headers: {
          Authorization: `Bearer ${tokens.accessToken}`,
        },
      });
      setTopics(response.data);
    } catch (error) {
      console.error("Error fetching topics:", error);
    }
  }

  useEffect(() => {
    fetchTopics(); // Initial fetch when component mounts
  }, []);

  const handleCreateTopic = async () => {
    console.log(moduleId);
    try {
      await axios.post(
        "/Topic",
        { topicName: topicName, moduleId: moduleId }, // Adjust topic name as needed
        {
          headers: {
            Authorization: `Bearer ${tokens.accessToken}`,
          },
        }
      );
      // After creating the topic, update the topic list
      fetchTopics();
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

  function saveTopicName(name) {
    setTopicName(name);
  }

  return (
    <>
      <div className="m-2">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><Link to="/ModulesManagment">Module Management</Link></li>
        <li class="breadcrumb-item active">Topic Management</li>
      </ol>
        <h1 className="text-center">Topic Management Panel</h1>
        <div className="input-group mb-3">
          <span
            className="input-group-text btn btn-secondary"
            id="inputGroup-sizing-default"
            onClick={handleCreateTopic}
          >
            Create topic
          </span>
          <input
            type="text"
            className="form-control"
            aria-label="Sizing example input"
            aria-describedby="inputGroup-sizing-default"
            placeholder="Enter topic name"
            onBlur={(e) => saveTopicName(e.target.value)}
          />
        </div>
        {createMessage && (
          <div className="alert alert-success">{createMessage}</div>
        )}
        {failCreateMessage && (
          <div className="alert alert-danger">{failCreateMessage}</div>
        )}
        <h4>Topic List</h4>
        <table className="table table-hover table-striped  table-bordered">
          <thead>
            <tr>
              <th scope="col">Module Id</th>
              <th scope="col">Topic Id</th>
              <th scope="col">Name</th>
              <th scope="col">Created by</th>
              <th scope="col">Created Time</th>
              <th scope="col">Manage Question</th>
              <th scope="col">Edit Name</th>
              <th scope="col">Delete</th>
            </tr>
          </thead>
          <tbody>
            {topics.map((topic) => (
              <Topic
                key={topic.topicId}
                topic={topic}
                fetchTopics={fetchTopics}
              />
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}
