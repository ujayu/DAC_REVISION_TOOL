import React, { useState } from "react";
import axios from "../../../axios";
import { useSelector } from "react-redux";
import { Link } from "react-router-dom";

export default function Topic({ topic, fetchTopics }) {
  const [editedTopicName, setEditedTopicName] = useState(topic.topicName);
  const [saveStatus, setSaveStatus] = useState("");
  const tokens = useSelector((state) => state.token);

  const handleEditName = async () => {
    try {
        console.log(editedTopicName);
      await axios.put(
        `/Topic/${topic.topicId}`,
        {
          topicName: editedTopicName,
        },
        {
          headers: {
            Authorization: `Bearer ${tokens.accessToken}`,
          },
        }
      );
      setSaveStatus("Name saved successfully!");
    } catch (error) {
      console.error("Error editing topic name:", error);
      setSaveStatus("Failed to save name");
    }
  };

  const handleDeleteTopic = async () => {
    try {
      await axios.delete(`/Topic/${topic.topicId}`, {
        headers: {
          Authorization: `Bearer ${tokens.accessToken}`,
        },
      });
      fetchTopics();
    } catch (error) {
      console.error("Error deleting topic:", error);
    }
  };

  const handleBlur = () => {
    handleEditName();
    setSaveStatus(""); // Clear the save status message after onBlur
  };

  return (
    <tr className="table-primary">
      <td>{topic.moduleId}</td>
      <td>{topic.topicId}</td>
      <td>{topic.topicName}</td>
      <td>{topic.createBy}</td>
      <td>{topic.createTime}</td>
      <td>
        <Link to={`/ManagePoint/${topic.topicId}`} className="btn btn-info">
          Manage Question
        </Link>
      </td>
      <td>
        <div className="input-group">
          <input
            type="text"
            className="form-control"
            value={editedTopicName}
            onChange={(e) => setEditedTopicName(e.target.value)}
            onBlur={handleBlur}
          />
          <button
            type="button"
            className="btn btn-success"
            onClick={handleEditName}
          >
            Save
          </button>
        </div>
        <div className="text-success mt-1">{saveStatus}</div>
      </td>
      <td>
        <button
          type="button"
          className="btn btn-danger"
          onClick={handleDeleteTopic}
        >
          Delete
        </button>
      </td>
    </tr>
  );
}
