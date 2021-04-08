import React, { useState, useEffect } from 'react'
import {FaTimes} from 'react-icons/fa'

const Category = ({ category, onCatEdit, onCatDelete }) => {
    const [enableSave, setEnableSave] = useState(false);
    const [newCat, setNewCat] = useState(category);
    const [catChanged, setCatChanged] = useState(false);
    const [imSource, setImageSource] = useState(category.imSource);

    const nameChanged = n => {
        if (!newCat) {
            var nC = {
                category_ID: category.category_ID,
                name: n,
                min: category.min,
                max: category.max,
                imSource: category.imSource,
                photoFile: category.photoFile
            }
        }
        else {
            var nC = {
                category_ID: newCat.category_ID,
                name: n,
                min: newCat.min,
                max: newCat.max,
                imSource: newCat.imSource,
                photoFile: newCat.photoFile
            }
        }
        setNewCat(nC);
        setCatChanged(true);
    }

    const imSourceChanged = imS => {
        if (!newCat) {
            var nC = {
                category_ID: category.category_ID,
                name: category.name,
                min: category.min,
                max: category.max,
                imSource: imS,
                photoFile: category.photoFile
            }
        }
        else {
            var nC = {
                category_ID: newCat.category_ID,
                name: newCat.name,
                min: newCat.min,
                max: newCat.max,
                imSource: imS,
                photoFile: newCat.photoFile
            }
        }
        setImageSource(imS);
        setNewCat(nC);
        setCatChanged(true);
    }

    const cancelChanges = () => {
        setEnableSave(false);
        setCatChanged(false);
        setNewCat(category);
    }

    const saveChanges = () => {
        if (catChanged) {
            if (!newCat.name) {
                alert('Please enter a name');
                return;
            }
            if (!newCat.imSource) {
                alert('Please upload icon');
                return; 
            }
            var im = document.getElementById(category.category_ID);
            onCatEdit(newCat,im);
        }
        setNewCat(category);
        setEnableSave(!enableSave);
        setCatChanged(false);
    }
        
    

    return (
        <div className='category'>

            <h3>{category.name} <FaTimes style={{ color: 'red', cursor: 'pointer' }} onClick={() => onCatDelete(category.category_ID)} /> </h3>
            <div className='card-container'>
                <div className='card'>
                    <p>Category name:</p>
                    <p>Minimum weight:</p>
                    <p>Maximum weight:</p>   
                </div>
                <div className='card'>
                    {enableSave ?
                        <p><input type='text' defaultValue={category.name}
                            onChange={(e) => nameChanged(e.target.value)} /></p> :
                        <p><label>{category.name}</label></p>
                    }
                    <p><label>{category.min}</label></p>
                    <p><label>{category.max}</label></p>   
                </div>
                <div className='card' style={{ border: '1px solid black' }}>
                    <div style={{margin:'0px'}}>
                        <img width='40%' src={imSource} alt='Category icon' key={category.imSource} /> 
                    </div>
                    <div>
                        <input type='file' id={category.category_ID} disabled={!enableSave}
                            onChange={(imURL) => {
                                var file = document.getElementById(category.category_ID);
                                var reader = new FileReader(); 
                                reader.onload = function (e) {
                                    imSourceChanged(e.target.result);
                                };
                                reader.onerror = function (e) {
                                    console.log("cannot read file");
                                };
                                reader.readAsDataURL(file.files[0]);
                            }}
                            />
                    </div>                    
                </div>
            </div>
            <div className='card-container'>
                <div className='card'>
                    <button className='btn btn-block' onClick={saveChanges} >{!enableSave ? 'Edit' : 'Save Changes'}</button>
                </div>
                <div className='card'>
                    <button className={enableSave ? 'btnc btn-block' : 'btnd btn-block'} disabled={!enableSave} onClick={()=>cancelChanges()} > Cancel </button>
                </div>
            </div>
        </div>
    )
}

export default Category
