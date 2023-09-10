import React, { useState } from "react";
import Module from "./ModulesManagment/Module";
import axios from "./../../axios"; // Make sure you've imported the API properly
import { useSelector } from "react-redux";

export default function ModulesManagment() {
  // Define state to hold modules data
  const [modules, setModules] = useState([]);
  const [createMessage, setCreateMessage] = useState("");
  const [failCreateMessage, setFailCreateMessage] = useState("");
  const [moduleName, setModuleName] = useState("");
  const tokens = useSelector((state) => state.token);

  // Fetch modules data when component mounts
  async function fetchModules() {
    try {
      const response = await axios.get("/Module/All", {
        headers: {
          Authorization: `Bearer ${tokens.accessToken}`,
        },
      });
      setModules(response.data);
    } catch (error) {
      console.error("Error fetching modules:", error);
    }
  }

  React.useEffect(() => {
    fetchModules(); // Initial fetch when component mounts
  }, []);

  const handleCreateModule = async () => {
    try {
      await axios.post(
        "/Module",
        { moduleName: moduleName }, // Adjust module name as needed
        {
          headers: {
            Authorization: `Bearer ${tokens.accessToken}`,
          },
        }
      );
      // After creating the module, update the module list
      fetchModules();
      setCreateMessage("Module created successfully!");
      setTimeout(()=>{
        setCreateMessage("");
      },2000)
    } catch (error) {
      console.log("Error creating module:", error);
      setFailCreateMessage(error.response.data.message);
      setTimeout(()=>{
        setFailCreateMessage("");
      },2000)
    }
  };

  function saveModuleName(name) {
    setModuleName(name)
  }

  return (
    <>
      <div className="m-2">
        <h1 className="text-center">Module Management Panel</h1>
        <div className="input-group mb-3">
          <span
            className="input-group-text btn btn-secondary"
            id="inputGroup-sizing-default"
            onClick={handleCreateModule}
          >
            Create module
          </span>
          <input
            type="text"
            className="form-control"
            aria-label="Sizing example input"
            aria-describedby="inputGroup-sizing-default"
            placeholder="Enter module name"
            onBlur={(e)=>saveModuleName(e.target.value)}
          />
        </div>
        {createMessage && (
          <div className="alert alert-success">{createMessage}</div>
        )}
        {failCreateMessage && (
          <div className="alert alert-danger">{failCreateMessage}</div>
        )}
        <h4>Module List</h4>
        <table className="table table-hover table-striped  table-bordered">
          <thead>
            <tr>
              <th scope="col">Module Id</th>
              <th scope="col">Name</th>
              <th scope="col">Create By</th>
              <th scope="col">Add Question</th>
              <th scope="col">Edit Name</th>
              <th scope="col">Delete</th>
            </tr>
          </thead>
          <tbody>
            {modules.map((module) => (
              <Module
                key={module.moduleId}
                module={module}
                fetchModules={fetchModules}
              />
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}
