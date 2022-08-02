import * as express from 'express';
import dbClass from "sap-hdbext-promisfied";
import * as hdbext from '@sap/hdbext';

export const loadRoutes = (app) => {
    app.get("/api/vendors", async (/** @type {express.Request} */ req, /** @type {express.Response} */ res) => {
        try {
            let db = new dbClass(req.db)
            const statement = await db.preparePromisified('select * from "VENDOR"')
            const results = await db.statementExecPromisified(statement, [])
            return res.type("application/json").status(200).send(results)
        } catch (e) {
            return res.type("text/plain").status(500).send(`ERROR: ${e.toString()}`)
        }
    })
}