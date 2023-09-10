import React from "react";
import API from "../../../axios";
import { useSelector } from "react-redux";
import { Link } from "react-router-dom";

export default function Module({ module, fetchModules }) {
  const [editedModuleName, setEditedModuleName] = React.useState(
    module.moduleName
  );
  const [saveStatus, setSaveStatus] = React.useState("");
  const tokens = useSelector((state) => state.token);

  const handleEditName = async () => {
    try {
      await API.put(
        `/Module/${module.moduleId}`,
        {
          moduleName: editedModuleName,
        },
        {
          headers: {
            Authorization: `Bearer ${tokens.accessToken}`,
          },
        }
      );
      setSaveStatus("Name saved successfully!");
      fetchModules();
    } catch (error) {
      console.error("Error editing module name:", error);
      setSaveStatus("Failed to save name");
    }
  };

const handleDeleteModule = async () => {
  try {
    await API.delete(`/Module/${module.moduleId}`, {
      headers: {
        Authorization: `Bearer ${tokens.accessToken}`,
      },
    });

    // Refresh module list after successful deletion
    fetchModules();
  } catch (error) {
    console.error("Error deleting module:", error);
  }
};


  const handleBlur = () => {
    handleEditName();
    setSaveStatus(""); // Clear the save status message after onBlur
  };

  return (
    <tr className="table-primary">
      <td>{module.moduleId}</td>
      <td>{module.moduleName}</td>
      <td>{module.createdBy}</td>
      <td>
        <Link to={`/ManageModule/${module.moduleId}`} className="btn btn-info">
          Manage Topic
        </Link>
      </td>
      <td>
        <div className="input-group">
          <input
            type="text"
            className="form-control"
            value={editedModuleName}
            onChange={(e) => setEditedModuleName(e.target.value)}
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
          onClick={handleDeleteModule}
        >
          Delete
        </button>
      </td>
    </tr>
  );
}
