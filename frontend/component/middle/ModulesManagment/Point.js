import React, { useState } from "react";
import axios from "../../../axios";
import { useSelector } from "react-redux";

export default function Point({ point, fetchPoints }) {
  const [editedPointValue, setEditedPointValue] = useState(point.point1);
  const [editedDescription, setEditedDescription] = useState(point.description);
  const [saveStatus, setSaveStatus] = useState("");
  const tokens = useSelector((state) => state.token);

  const handleEditDetails = async () => {
    try {
      await axios.put(
        `/Point/${point.pointId}`,
        {
            topicId:point.topicId,
          point1: editedPointValue,
          description: editedDescription,
        },
        {
          headers: {
            Authorization: `Bearer ${tokens.accessToken}`,
          },
        }
      );
      setSaveStatus("Details saved successfully!");
      fetchPoints();
    } catch (error) {
      console.error("Error editing point details:", error);
      setSaveStatus("Failed to save details");
    }
  };

  const handleBlur = () => {
    handleEditDetails();
    setSaveStatus(""); // Clear the save status message after onBlur
  };

  const handleDeletePoint = async () => {
    try {
      await axios.delete(`/Point/${point.pointId}`, {
        headers: {
          Authorization: `Bearer ${tokens.accessToken}`,
        },
      });

      // Refresh topic list after successful deletion
      fetchPoints();
    } catch (error) {
      console.error("Error deleting topic:", error);
    }
  };


  return (
    <tr className="table-primary">
      <td>{point.pointId}</td>
      <td>{point.topicId}</td>
      <td>{point.point1}</td>
      <td>{point.description}</td>
      <td>{point.createBy}</td>
      <td>{point.createTime}</td>
      <td>
        <input
            type="text"
            className="form-control"
            value={editedPointValue}
            onChange={(e) => setEditedPointValue(e.target.value)}
          />
      </td>
      <td>
        <div className="input-group">
          <input
            type="text"
            className="form-control"
            value={editedDescription}
            onChange={(e) => setEditedDescription(e.target.value)}
            onBlur={handleBlur}
          />
          <button
            type="button"
            className="btn btn-success"
            onClick={handleEditDetails}
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
          onClick={() => handleDeletePoint(point.pointId)}
        >
          Delete
        </button>
      </td>
    </tr>
  );
}
