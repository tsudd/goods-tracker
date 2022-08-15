import * as express from 'express';
import dbClass from "sap-hdbext-promisfied";

export const loadRoutes = (app) => {
    app.get("/vendors", async (/** @type {express.Request} */ req, /** @type {express.Response} */ res) => {
        console.log(req)
        try {
            let db = new dbClass(req.db)
            console.log(db)
            const statement = await db.preparePromisified('select * from "VENDOR"')
            const results = await db.statementExecPromisified(statement, [])
            return res.type("application/json").status(200).send(results)
        } catch (e) {
            return res.type("text/plain").status(500).send(`ERROR: ${e.toString()}`)
        }
    })
}